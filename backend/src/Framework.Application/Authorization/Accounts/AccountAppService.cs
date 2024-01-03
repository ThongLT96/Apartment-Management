using System;
using System.Threading.Tasks;
using System.Web;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Framework.Authorization.Accounts.Dto;
using Framework.Authorization.Impersonation;
using Framework.Authorization.Users;
using Framework.Configuration;
using Framework.Debugging;
using Framework.MultiTenancy;
using Framework.Security.Recaptcha;
using Framework.Url;
using Framework.Authorization.Delegation;
using Abp.Domain.Repositories;
using Abp.Net.Mail;
using static System.Net.WebRequestMethods;
using NPOI.SS.Formula.Functions;
using Twilio.TwiML.Messaging;
using System.Net.Mail;
using Castle.MicroKernel.Util;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;

namespace Framework.Authorization.Accounts
{
    public class AccountAppService : FrameworkAppServiceBase, IAccountAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        public IRecaptchaValidator RecaptchaValidator { get; set; }

        private readonly IUserEmailer _userEmailer;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IUserLinkManager _userLinkManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IWebUrlService _webUrlService;
        private readonly IUserDelegationManager _userDelegationManager;
        private readonly IAbpSession _session;
        private readonly IRepository<EmailAddressOTP> _emailAddressOTPRepository;

        private const string _cloneEmailPrefix = "emailliame123456123qwe";
        private const string _cloneEmailSuffix = "@yopmail.com";
        private string _cloneOTP = SimpleStringCipher.Instance.Encrypt("emailliame123456123qwe");

        public AccountAppService(
            IUserEmailer userEmailer,
            UserRegistrationManager userRegistrationManager,
            IImpersonationManager impersonationManager,
            IUserLinkManager userLinkManager,
            IPasswordHasher<User> passwordHasher,
            IWebUrlService webUrlService,
            IUserDelegationManager userDelegationManager,
            IAbpSession session,
            IRepository<EmailAddressOTP> emailAddressOTPRepository)
        {
            _userEmailer = userEmailer;
            _userRegistrationManager = userRegistrationManager;
            _impersonationManager = impersonationManager;
            _userLinkManager = userLinkManager;
            _passwordHasher = passwordHasher;
            _webUrlService = webUrlService;

            AppUrlService = NullAppUrlService.Instance;
            RecaptchaValidator = NullRecaptchaValidator.Instance;
            _userDelegationManager = userDelegationManager;
            _session = session;
            _emailAddressOTPRepository = emailAddressOTPRepository;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id, _webUrlService.GetServerRootAddress(input.TenancyName));
        }

        public Task<int?> ResolveTenantId(ResolveTenantIdInput input)
        {
            if (string.IsNullOrEmpty(input.c))
            {
                return Task.FromResult(AbpSession.TenantId);
            }

            var parameters = SimpleStringCipher.Instance.Decrypt(input.c);
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["tenantId"] == null)
            {
                return Task.FromResult<int?>(null);
            }

            var tenantId = Convert.ToInt32(query["tenantId"]) as int?;
            return Task.FromResult(tenantId);
        }

        /*
        *   01-2024 Lo Tri Thong
        *   From here
        */

        public async Task SendEmailActivationOTP(RegisterInput input)
        {
            var user = await UserManager.FindByEmailAsync(input.EmailAddress);

            if (user == null)
            {
                user = await SaveUserDataAsync(input);
            }
            else
            {
                if (user.IsEmailConfirmed)
                {
                    throw new UserFriendlyException("Email đã được người khác đăng ký!");
                }
                else
                {
                    if (IsCloneEmail(input.EmailAddress))
                    {
                        user.EmailConfirmationCode = _cloneOTP;
                    }
                    else
                    {
                        user.EmailConfirmationCode = GenerateOTP();
                    }
                }
            }

            if (!IsCloneEmail(input.EmailAddress))
            {
                await _userEmailer.SendEmailActivationOTPAsync(user);
            }
        }

        public async Task SendEmailAddressOTP(RegisterByEmailInput input)
        {
            // kiểm tra tenant

            // nếu email tồn tại
            var checkEmailExistInput = new CheckEmailExistInput { EmailAddress = input.EmailAddress };
            if (await EmailAddressIsExist(checkEmailExistInput))
            {
                throw new UserFriendlyException("Email này đã tồn tại!");
            }
            else
            {
                var emailAddressOTP = new EmailAddressOTP
                {
                    EmailAddress = input.EmailAddress,
                    OTP = GenerateOTP(),
                    IsConfirmed = false,
                };

                // kiểm tra tồn tại, nếu tồn tại rồi thì xóa luôn cho đỡ chật db
                var data = await _emailAddressOTPRepository.FirstOrDefaultAsync(d => d.EmailAddress == emailAddressOTP.EmailAddress);
                if (data != null)
                {
                    await _emailAddressOTPRepository.HardDeleteAsync(data);
                }

                // insert
                await _emailAddressOTPRepository.InsertAsync(emailAddressOTP);

                await _userEmailer.SendOTPToEmailAsync(emailAddressOTP);
            }
        }

        public async Task<ConfirmEmailAddressOTPOutput> ConfirmEmailAddressOTP(ConfirmEmailAddressOTPInput input)
        {
            // kiểm tra tenant

            // lấy thông tin email và OTP
            var data = await _emailAddressOTPRepository.FirstOrDefaultAsync(d => d.EmailAddress == input.EmailAddress && !d.IsConfirmed);

            if (data == null)
            {
                throw new UserFriendlyException("Chưa gửi OTP đến email này");
            }

            if (SimpleStringCipher.Instance.Decrypt(data.OTP) != input.OTP)
            {
                data.IsConfirmed = false;
                return new ConfirmEmailAddressOTPOutput
                {
                    IsCorrect = data.IsConfirmed,
                    Message = "OTP không chính xác!"
                };
            }

            data.IsConfirmed = true;
            await _emailAddressOTPRepository.UpdateAsync(data);

            return new ConfirmEmailAddressOTPOutput
            {
                IsCorrect = data.IsConfirmed,
                Message = "Xác thực email thành công!"
            };
        }

        public async Task<RegisterOutput> RegisterByEmailAddress(RegisterInput input)
        {
            // kiểm tra tenant

            // check email đã confirm chưa
            var emailConfirmationData = await _emailAddressOTPRepository.FirstOrDefaultAsync(d => d.EmailAddress == input.EmailAddress);
            if (!emailConfirmationData.IsConfirmed)
            {
                throw new UserFriendlyException("Email chưa được xác thực");
            }

            // check user đã tồn tại chưa (cho chắc, không thừa đâu)
            var user = await UserManager.FindByEmailAsync(input.EmailAddress);
            if (user != null && !user.IsDeleted)
            {
                throw new UserFriendlyException("Email đã tồn tại");
            }

            // check AgreeToTermsAndConditions
            if (!input.AgreeToTermsAndConditions)
            {
                throw new UserFriendlyException("Để hoàn tất việc đăng ký tài khoản Apato, bạn vui lòng đọc và chấp nhận điều khoản của chúng tôi.");
            }

            // Đăng ký user mới
            user = await _userRegistrationManager.RegisterAsync(
                        input.EmailAddress,
                        input.Password,
                        input.FullName,
                        input.Gender,
                        input.IDNumber,
                        input.BirthDate,
                        input.BuildingId,
                        input.ApartmentId,
                        false, "", input.PhoneNumber);
            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;
            CheckErrors(await UserManager.UpdateAsync(user));

            return new RegisterOutput { CanLogin = true };
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await UserManager.FindByEmailAsync(input.EmailAddress);
            var output = new RegisterOutput { CanLogin = false };

            if (user != null)
            {
                if (!input.AgreeToTermsAndConditions)
                {
                    throw new UserFriendlyException("Để hoàn tất việc đăng ký tài khoản Apato, bạn vui lòng đọc và chấp nhận điều khoản của chúng tôi.");
                }

                if (!input.OTP.Equals(SimpleStringCipher.Instance.Decrypt(user.EmailConfirmationCode)))
                {
                    throw new UserFriendlyException("OTP không đúng!");
                }

                await ConfirmUserData(user, input);

                output.CanLogin = true;
            }
            else
            {
                throw new UserFriendlyException("OTP chưa được gửi!");
            }

            return output;
        }

        public async Task<RegisterOutput> RegisterByPhoneNumber(RegisterInput input)
        {
            input.EmailAddress = _cloneEmailPrefix + input.PhoneNumber.Trim() + _cloneEmailSuffix;

            var user = await UserManager.FindByEmailAsync(input.EmailAddress);
            if (user != null && user.IsEmailConfirmed)
            {
                throw new UserFriendlyException("Số điện thoại đã được sử dụng, vui lòng thử lại số khác!");
            }

            await SendEmailActivationOTP(input);
            input.OTP = SimpleStringCipher.Instance.Decrypt(_cloneOTP);
            var output = await Register(input);

            return output;
        }

        public async Task<bool> EmailAddressIsExist(CheckEmailExistInput input)
        {
            // thực tế là phải kiểm tra tenant nữa, nhưng mà thôi

            var user = await UserManager.FindByEmailAsync(input.EmailAddress);
            if (user == null)
            {
                return false;
            }

            if (user.IsDeleted == true)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> PhoneNumberIsExist(CheckPhoneNumberExistInput input)
        {
            // thực tế là phải kiểm tra tenant nữa, nhưng mà thôi

            var user = await UserManager.Users.FirstOrDefaultAsync<User>(u => u.PhoneNumber == input.PhoneNumber);
            if (user == null)
            {
                return false;
            }

            if (user.IsDeleted == true)
            {
                return false;
            }

            return true;
        }

        public async Task SendPasswordResetOTP(SendPasswordResetCodeInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);
            user.PasswordResetCode = GenerateOTP();

            await _userEmailer.SendPasswordResetOTPAsync(user);
        }

        public async Task<CheckPasswordResetOTPOutput> CheckPasswordResetOTP(CheckPasswordResetOTPInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);

            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || SimpleStringCipher.Instance.Decrypt(user.PasswordResetCode) != input.OTP)
            {
                return new CheckPasswordResetOTPOutput
                {
                    OTPIsCorrect = false,
                    Message = "Xác nhận Email thất bại."
                };
            }

            return new CheckPasswordResetOTPOutput
            {
                OTPIsCorrect = true,
                EmailAddress = user.EmailAddress
            };
        }

        public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);

            if (user == null)
            {
                throw new UserFriendlyException("Không xác định được tài khoản.");
            }

            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        public async Task<ResetPasswordOutput> ResetPasswordWithPhoneNumber(ResetPasswordWithPhoneNumberInput input)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == input.PhoneNumber);
            if (user == null)
            {
                throw new UserFriendlyException("Không xác định được tài khoản.");
            }

            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
            user.PasswordResetCode = null;
            user.IsPhoneNumberConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }
        /*
        *   01-2024 Lo Tri Thong
        *   To here
        */

        public async Task SendEmailActivationLink(SendEmailActivationLinkInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(
                user,
                AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId)
            );
        }

        public async Task ActivateEmail(ActivateEmailInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user != null && user.IsEmailConfirmed)
            {
                return;
            }

            if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() || user.EmailConfirmationCode != input.ConfirmationCode)
            {
                throw new UserFriendlyException(L("InvalidEmailConfirmationCode"), L("InvalidEmailConfirmationCode_Detail"));
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;

            await UserManager.UpdateAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Impersonation)]
        public virtual async Task<ImpersonateOutput> Impersonate(ImpersonateInput input)
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationToken(input.UserId, input.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
            };
        }

        public virtual async Task<ImpersonateOutput> DelegatedImpersonate(DelegatedImpersonateInput input)
        {
            var userDelegation = await _userDelegationManager.GetAsync(input.UserDelegationId);
            if (userDelegation.TargetUserId != AbpSession.GetUserId())
            {
                throw new UserFriendlyException("User delegation error.");
            }

            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationToken(userDelegation.SourceUserId, userDelegation.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(userDelegation.TenantId)
            };
        }

        public virtual async Task<ImpersonateOutput> BackToImpersonator()
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetBackToImpersonatorToken(),
                TenancyName = await GetTenancyNameOrNullAsync(AbpSession.ImpersonatorTenantId)
            };
        }

        public virtual async Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input)
        {
            if (!await _userLinkManager.AreUsersLinked(AbpSession.ToUserIdentifier(), input.ToUserIdentifier()))
            {
                throw new Exception(L("This account is not linked to your account"));
            }

            return new SwitchToLinkedAccountOutput
            {
                SwitchAccountToken = await _userLinkManager.GetAccountSwitchToken(input.TargetUserId, input.TargetTenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TargetTenantId)
            };
        }

        private bool UseCaptchaOnRegistration()
        {
            //return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration);
            return false;   // set false luôn khỏi setting cái gì hết
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await TenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        private async Task<string> GetTenancyNameOrNullAsync(int? tenantId)
        {
            return tenantId.HasValue ? (await GetActiveTenantAsync(tenantId.Value)).TenancyName : null;
        }

        private async Task<User> GetUserByChecking(string inputEmailAddress)
        {
            var user = await UserManager.FindByEmailAsync(inputEmailAddress);
            if (user == null)
            {
                throw new UserFriendlyException(L("InvalidEmailAddress"));
            }

            return user;
        }

        /*
        *   01-2024 Lo Tri Thong
        *   From here
        */
        private async Task<User> SaveUserDataAsync(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                        input.EmailAddress,
                        input.Password,
                        input.FullName,
                        input.Gender,
                        input.IDNumber,
                        input.BirthDate,
                        input.BuildingId,
                        input.ApartmentId,
                        false,
                        input.PhoneNumber.IsNullOrEmpty() ? GenerateOTP() : _cloneOTP,
                        input.PhoneNumber);
            return user;
        }

        private async Task ConfirmUserData(User user, RegisterInput input)
        {
            string[] _fullName = _userRegistrationManager.SplitedFullName(input.FullName);

            user.Name = _fullName[1];
            user.Surname = _fullName[0];
            user.Gender = input.Gender;
            user.IDNumber = input.IDNumber;
            user.BirthDate = input.BirthDate;

            user.BuildingId = input.BuildingId;
            user.ApartmentId = input.ApartmentId;

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;
            if (IsCloneEmail(input.EmailAddress))
            {
                user.IsPhoneNumberConfirmed = true;
            }

            CheckErrors(await UserManager.UpdateAsync(user));

            if (!input.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        private string GenerateOTP()
        {
            // generate a random 6-digit otp
            Random _r = new Random();
            return SimpleStringCipher.Instance.Encrypt(_r.Next(100000, 1000000).ToString());
        }

        public static bool IsCloneEmail(string email) {
            if (email.Contains(_cloneEmailPrefix) && email.Contains(_cloneEmailSuffix))
            {
                return true;
            }
            return false;
        }

        public async Task<CheckEmailCloneOutput> EmailAddressIsClone(CheckEmailCloneInput input)
        {
            if (input.EmailAddress.Contains(_cloneEmailPrefix) && input.EmailAddress.Contains(_cloneEmailSuffix))
            {
                return new CheckEmailCloneOutput
                {
                    IsClone = true,
                    Message = "Email chưa cập nhật"
                };
            }

            return new CheckEmailCloneOutput
            {
                IsClone = false,
                Message = "Đây là email thiệt"
            };
        }

        // Thong's personal use
        public async Task<string> GiveMeThisStringEncrypted(PersonalUseInput input)
        {
            return SimpleStringCipher.Instance.Encrypt(input.PlainText);
        }
        
        /*
        *   01-2024 Lo Tri Thong
        *   To here
        */
    }

    public class PersonalUseInput
    { 
        public string PlainText { get; set; }
    }
}