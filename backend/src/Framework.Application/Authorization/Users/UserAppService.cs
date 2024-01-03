using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Framework.Authorization.Permissions;
using Framework.Authorization.Permissions.Dto;
using Framework.Authorization.Roles;
using Framework.Authorization.Users.Dto;
using Framework.Authorization.Users.Exporting;
using Framework.Dto;
using Framework.Notifications;
using Framework.Url;
using Framework.Organizations.Dto;
using Microsoft.AspNetCore.Mvc.Filters;
using Z.EntityFramework.Plus;
using Castle.Core.Internal;
using Abp.Domain.Uow;
using Framework.ServiceRegister;
using Framework.UserServiceRegister.Dto;
using Framework.Bill;
using Framework.Bill.Dto;
using Framework.Authorization.Accounts;

namespace Framework.Authorization.Users
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UserAppService : FrameworkAppServiceBase, IUserAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        private readonly RoleManager _roleManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUserListExcelExporter _userListExcelExporter;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IUserPolicy _userPolicy;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRoleManagementConfig _roleManagementConfig;
        private readonly UserManager _userManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserServiceRegister2> _userServiceRepository;
        private readonly IRepository<ApartmentBill> _apartmentBillRepository;
        private readonly IRepository<ServiceBill> _serviceBillRepository;


        public UserAppService(
            RoleManager roleManager,
            IUserEmailer userEmailer,
            IUserListExcelExporter userListExcelExporter,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IUserPolicy userPolicy,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRoleManagementConfig roleManagementConfig,
            UserManager userManager,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository,
            IRepository<User, long> userRepository,
            IRepository<UserServiceRegister2> userServiceRepository,
            IRepository<ApartmentBill> apartmentBillRepository,
            IRepository<ServiceBill> serviceBillRepository)
        {
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userListExcelExporter = userListExcelExporter;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _userRoleRepository = userRoleRepository;
            _userPolicy = userPolicy;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;
            _organizationUnitRepository = organizationUnitRepository;
            _roleManagementConfig = roleManagementConfig;
            _userManager = userManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userServiceRepository = userServiceRepository;
            _apartmentBillRepository = apartmentBillRepository;
            _serviceBillRepository = serviceBillRepository;

            AppUrlService = NullAppUrlService.Instance;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            var query = GetUsersFilteredQuery(input);

            var userCount = await query.CountAsync();

            var users = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
            );
        }

        public async Task<FileDto> GetUsersToExcel(GetUsersToExcelInput input)
        {
            var query = GetUsersFilteredQuery(input);

            var users = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return _userListExcelExporter.ExportToFile(userListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            //Getting all available roles
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync();

            var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                AllOrganizationUnits = ObjectMapper.Map<List<OrganizationUnitDto>>(allOrganizationUnits),
                MemberedOrganizationUnits = new List<string>()
            };

            if (!input.Id.HasValue)
            {
                //Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled =
                        await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement
                            .TwoFactorLogin.IsEnabled),
                    IsLockoutEnabled =
                        await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut
                            .IsEnabled)
                };

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = ObjectMapper.Map<UserEditDto>(user);
                output.ProfilePictureId = user.ProfilePictureId;

                var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
                output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();

                var allRolesOfUsersOrganizationUnits = GetAllRoleNamesOfUsersOrganizationUnits(input.Id.Value);

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                    userRoleDto.InheritedFromOrganizationUnit =
                        allRolesOfUsersOrganizationUnits.Contains(userRoleDto.RoleName);
                }
            }

            return output;
        }

        private List<string> GetAllRoleNamesOfUsersOrganizationUnits(long userId)
        {
            return (from userOu in _userOrganizationUnitRepository.GetAll()
                    join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
                        .OrganizationUnitId
                    join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
                    where userOu.UserId == userId
                    select userOuRoles.Name).ToList();
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName)
                    .ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var grantedPermissions =
                PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }

        public async Task CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {
            if (input.AssignedRoleNames.Contains("Manager") || input.AssignedRoleNames.Contains("Admin"))
            {
                //throw new UserFriendlyException("Chức năng này dùng cho ban quản lí, chỉ thao tác được với tài khoản Cư dân. Muốn thao tác với tài khoản ban quản lí hoặc ban quản tri, vui lòng đăng nhập tài khoản ban quản trị.");
            }

            if (input.User.Id.HasValue)
            {
                await UpdateUserAsync(input);
            }
            else
            {
                await CreateUserAsync(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ManageManagers)]
        public async Task CreateOrUpdateManager(CreateOrUpdateUserInput input)
        {
            if (input.User.Id.HasValue)
            {
                await UpdateUserAsync(input);
            }
            else
            {
                await CreateUserAsync(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Unlock)]
        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

        /*
        *   01-2024 Lo Tri Thong
        *   From here
        */
        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserByEmailOutput> GetUserByEmail(GetUserByEmailInput input)
        {
            var user = await UserManager.FindByEmailAsync(input.EmaillAddress);

            if (user == null)
            {
                throw new UserFriendlyException("Không tồn tại người dùng có email này");
            }

            string accountType;
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                accountType = "Ban quản trị";
            }
            else if (await _userManager.IsInRoleAsync(user, "Manager"))
            {
                accountType = "Ban quản lý";
            }
            else
            {
                accountType = "Cư dân";
            }

            return new GetUserByEmailOutput
            {
                Id = user.Id,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                EmailAddress = AccountAppService.IsCloneEmail(user.EmailAddress) ? "Chưa cập nhật" : user.EmailAddress,
                ApartmentId = user.ApartmentId,
                PhoneNumber = user.PhoneNumber.IsNullOrEmpty() ? "Chưa cập nhật" : user.PhoneNumber,
                IDNumber = user.IDNumber,
                ProfileAvatar = user.ProfileAvatar,
                AccountType = accountType
            };
        }

        /*
        *   01-2024 Lo Tri Thong
        *   To here
        */

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<ListResultDto<GetAllUsersOutput>> GetAllUsers()
        {
            var a = _userRepository.GetAll();
            return new ListResultDto<GetAllUsersOutput>(ObjectMapper.Map<List<GetAllUsersOutput>>(a));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<UserDetailsOutput> GetUserDetails(string email)
        {
            var a = await UserManager.FindByEmailAsync(email);
            var b = _userServiceRepository.GetAll().Where(p => p.EmailAddress == email);

            var d = _apartmentBillRepository.GetAll().Where(p => p.EmailAddress == email);
            var e = _serviceBillRepository.GetAll().Where(p => p.EmailAddress == email);
            return new UserDetailsOutput
            {
                Name = a.Name,
                Surname = a.Surname,
                EmailAddress = a.EmailAddress,
                Gender = a.Gender,
                BirthDate = a.BirthDate,
                IDNumber = a.IDNumber,
                ApartmentId = a.ApartmentId,
                UserServices = new ListResultDto<UserRegisterDto>(ObjectMapper.Map<List<UserRegisterDto>>(b)),
                UserApartmentBills = new ListResultDto<ApartmentBillListDto>(ObjectMapper.Map<List<ApartmentBillListDto>>(d)),
                UserServiceBills = new ListResultDto<ServiceBillListDto>(ObjectMapper.Map<List<ServiceBillListDto>>(e))
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
        protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            var user = await UserManager.FindByIdAsync(input.User.Id.Value.ToString());

            //Update user properties
            ObjectMapper.Map(input.User, user); //Passwords is not mapped (see mapping configuration)

            CheckErrors(await UserManager.UpdateAsync(user));

            if (input.SetRandomPassword)
            {
                var randomPassword = await _userManager.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
            }

            //Update roles
            CheckErrors(await UserManager.SetRolesAsync(user, input.AssignedRoleNames));

            //update organization units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        protected virtual async Task CreateUserAsync(CreateOrUpdateUserInput input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(AbpSession.GetTenantId());
            }

            var user = ObjectMapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)
            user.TenantId = AbpSession.TenantId;

            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = await _userManager.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
                }

                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);

            //Organization Units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            //Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }
        }

        [HttpPost]
        public async Task<GetUsersToApproveOutput> GetUsersToApprove()
        {
            // check tenant

            // lấy users chưa active từ database
            var users = await _userManager.Users.Where(u => u.IsActive == false && !u.IsDeleted).OrderBy(u => u.Id).ToListAsync();

            // lọc thông tin
            var filteredUsers = new List<UserApproveDto>();
            foreach (var user in users)
            {
                filteredUsers.Add(new UserApproveDto
                {
                    UserId = user.Id,
                    EmailAddress = user.EmailAddress,
                    ApartmentId = user.ApartmentId,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber
                });
            }

            return new GetUsersToApproveOutput { Users = filteredUsers };
        }
        // Role of CurrentUser
        public async Task<CurrentUserRoleOutput> GetRoleOfCurrentUser()
        {
            var currentUserId = AbpSession.GetUserId();
            //phai getall
            var currentUserRole = _userRoleRepository.GetAll().Where(r => r.UserId == currentUserId).ToArray();

            var role = await _roleManager.FindByIdAsync(currentUserRole[0].RoleId.ToString());

            return new CurrentUserRoleOutput { RoleId = currentUserRole[0].RoleId, DisplayName = role.DisplayName, Name = role.Name };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Approve)]
        public async Task ApproveUser(UserApproveDto approveData)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == approveData.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("Không tìm thấy thông tin tài khoản");
            }

            if (approveData.IsApproved)
            {
                user.IsActive = true;
            }
            else
            {
                user.IsActive = false;
                await _userRepository.HardDeleteAsync(user);
            }

            await _userEmailer.SendRegisterResponseEmail(
                user,
                approveData.RejectReasons.IsNullOrEmpty() ? null : approveData.RejectReasons);
        }

        private async Task FillRoleNames(IReadOnlyCollection<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */
            var userIds = userListDtos.Select(u => u.Id);

            var userRoles = await _userRoleRepository.GetAll()
                .Where(userRole => userIds.Contains(userRole.UserId))
                .Select(userRole => userRole).ToListAsync();

            var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();

            foreach (var user in userListDtos)
            {
                var rolesOfUser = userRoles.Where(userRole => userRole.UserId == user.Id).ToList();
                user.Roles = ObjectMapper.Map<List<UserListRoleDto>>(rolesOfUser);
            }

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role != null)
                {
                    roleNames[roleId] = role.DisplayName;
                }
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    if (roleNames.ContainsKey(userListRoleDto.RoleId))
                    {
                        userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                    }
                }

                userListDto.Roles = userListDto.Roles.Where(r => r.RoleName != null).OrderBy(r => r.RoleName).ToList();
            }
        }

        private IQueryable<User> GetUsersFilteredQuery(IGetUsersInput input)
        {
            var query = UserManager.Users
                .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
                .WhereIf(input.OnlyLockedUsers,
                    u => u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > DateTime.UtcNow)
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            if (input.Permissions != null && input.Permissions.Any(p => !p.IsNullOrWhiteSpace()))
            {
                var staticRoleNames = _roleManagementConfig.StaticRoles.Where(
                    r => r.GrantAllPermissionsByDefault &&
                         r.Side == AbpSession.MultiTenancySide
                ).Select(r => r.RoleName).ToList();

                input.Permissions = input.Permissions.Where(p => !string.IsNullOrEmpty(p)).ToList();

                var userIds = from user in query
                              join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                              from ur in urJoined.DefaultIfEmpty()
                              join urr in _roleRepository.GetAll() on ur.RoleId equals urr.Id into urrJoined
                              from urr in urrJoined.DefaultIfEmpty()
                              join up in _userPermissionRepository.GetAll()
                                  .Where(userPermission => input.Permissions.Contains(userPermission.Name)) on user.Id equals up.UserId into upJoined
                              from up in upJoined.DefaultIfEmpty()
                              join rp in _rolePermissionRepository.GetAll()
                                  .Where(rolePermission => input.Permissions.Contains(rolePermission.Name)) on
                                  new { RoleId = ur == null ? 0 : ur.RoleId } equals new { rp.RoleId } into rpJoined
                              from rp in rpJoined.DefaultIfEmpty()
                              where (up != null && up.IsGranted) ||
                                    (up == null && rp != null && rp.IsGranted) ||
                                    (up == null && rp == null && staticRoleNames.Contains(urr.Name))
                              group user by user.Id
                    into userGrouped
                              select userGrouped.Key;

                query = UserManager.Users.Where(e => userIds.Contains(e.Id));
            }

            return query;
        }

        /*
        *   01-2024 Lo Tri Thong
        *   From here
        */

        //Get list User Role 3 (Cư dân)
        public async Task<List<ListRoleUser>> GetAllRoleUser()
        {
            var u = _userRepository.GetAll().ToList();
            var ur = _userRoleRepository.GetAll().Where(r => r.RoleId == 3).ToList();
            var result = new List<ListRoleUser>();
            var role = await _roleRepository.FirstOrDefaultAsync(s => s.Id == 3);
            foreach (var item in ur)
            {
                var b = await _userRepository.FirstOrDefaultAsync(item.UserId);
                var a = new ListRoleUser();
                a.Id = (int)b.Id;
                a.FullName = b.Surname +" "+ b.Name;
                a.Password = b.Password;
                a.BirthDate = b.BirthDate;
                a.Gender = b.Gender;
                a.TypeAccount = role.DisplayName;
                a.EmailAddress = b.EmailAddress;
                a.ApartmentId = b.ApartmentId;
                a.PhoneNumber = b.PhoneNumber;
                a.IDNumber = b.IDNumber;
                a.ProfileAvatar = b.ProfileAvatar;
                result.Add(a);
            }
            return new List<ListRoleUser>(result);
        }

        //Trả về danh dách loại tài khoản Ban quản lý
        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ManageManagers)]
        public async Task<List<ListRoleUser>> GetAllManagement()
        {
            var u = _userRepository.GetAll().ToList();
            var ur = _userRoleRepository.GetAll().Where(r => r.RoleId == 8).ToList();
            var result = new List<ListRoleUser>();
            var role = await _roleRepository.FirstOrDefaultAsync(s => s.Id == 8);
            foreach (var item in ur)
            {
                var b = await _userRepository.FirstOrDefaultAsync(item.UserId);
                var a = new ListRoleUser();
                a.Id = (int)b.Id;
                a.FullName = b.Surname +" "+ b.Name;
                a.Password = b.Password;
                a.BirthDate = b.BirthDate;
                a.Gender = b.Gender;
                a.TypeAccount = role.DisplayName;
                a.EmailAddress = b.EmailAddress;
                a.ApartmentId = b.ApartmentId;
                a.PhoneNumber = b.PhoneNumber;
                a.IDNumber = b.IDNumber;
                a.ProfileAvatar = b.ProfileAvatar;
                result.Add(a);
            }
            return new List<ListRoleUser>(result);
        }

        //Trả về danh sách Ban quản trị
        public async Task<List<ListRoleUser>> GetAllBoard()
        {
            var u = _userRepository.GetAll().ToList();
            var ur = _userRoleRepository.GetAll().Where(r => r.RoleId == 2).ToList();
            var result = new List<ListRoleUser>();
            var role = await _roleRepository.FirstOrDefaultAsync(s => s.Id == 2);
            foreach (var item in ur)
            {
                var b = await _userRepository.FirstOrDefaultAsync(item.UserId);
                var a = new ListRoleUser();
                a.Id = (int)b.Id;
                a.FullName = b.Surname +" "+ b.Name;
                a.Password = b.Password;
                a.BirthDate = b.BirthDate;
                a.Gender = b.Gender;
                a.TypeAccount = role.DisplayName;
                a.EmailAddress = b.EmailAddress;
                a.ApartmentId = b.ApartmentId;
                a.PhoneNumber = b.PhoneNumber;
                a.IDNumber = b.IDNumber;
                a.ProfileAvatar = b.ProfileAvatar;
                result.Add(a);
            }
            return new List<ListRoleUser>(result);
        }
        /*
        *   01-2024 Lo Tri Thong
        *   To here
        */
    }
}
