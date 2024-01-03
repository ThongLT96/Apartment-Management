using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Bill.Dto;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Framework.AService;
using Framework.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Framework.AService.Dto;
using Framework.Authorization.Users;
using Framework.Authorization.Users.Dto;
using Framework.UserServiceRegister;
using Framework.ServiceRegister;
using Abp.Runtime.Session;
using Framework.ApartmentManagement;
using Abp.Authorization.Users;
using Framework.Authorization.Roles;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;

namespace Framework.Bill
{
    //[AbpAuthorize(AppPermissions.Pages_Tenant_Bill)]
    public class BillAppService : FrameworkAppServiceBase, IBillAppService
    {

        private readonly IRepository<ServiceBill> _serviceBillRepository;
        private readonly IRepository<ApartmentBill> _apartmentBillRepository;
        private readonly IRepository<ApartmentService> _apartmentServiceRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<BillingInformation> _billingInformationRepository;
        private readonly IRepository<UserServiceRegister2> _useSRRepository;
        private readonly IRepository<Apartment> _apartmentRepository;
        public UserManager _UserManager { get; set; }
        public IAbpSession _AbpSession { get; set; }

        public BillAppService(

            IRepository<ServiceBill> serviceBillRepository,
            IRepository<ApartmentBill> apartmentBillRepository,
            IRepository<ApartmentService> apartmentServiceRepository,
            IRepository<User, long> userRepository,
            IRepository<BillingInformation> billingInformation,
            IRepository<UserServiceRegister2> userSRRepository,
            IRepository<Apartment> apartmentRepository,
            UserManager UserManager,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IAbpSession abpSession)
        {

            _serviceBillRepository = serviceBillRepository;
            _apartmentBillRepository = apartmentBillRepository;
            _apartmentServiceRepository = apartmentServiceRepository;
            _userRepository = userRepository;
            _billingInformationRepository = billingInformation;
            _useSRRepository = userSRRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _apartmentRepository = apartmentRepository;
            _UserManager = UserManager;
            _AbpSession = AbpSession;
        }

        //////Create

        //[AbpAuthorize(AppPermissions.Pages_Tenant_Bill_CreateApartmentBill)]
        //ApartmentBill
        public async Task CreateApartmentBill_EW(CreateApartmentBill input)
        {
            var a = ObjectMapper.Map<ApartmentBill>(input);
            await _apartmentBillRepository.InsertAsync(a);
        }
        public async Task CreateApartmentBill_Manage(CreateApartmentBill_Manage input)
        {
            var a = ObjectMapper.Map<ApartmentBill>(input);
            await _apartmentBillRepository.InsertAsync(a);
        }
        //ServiceBill
        //[AbpAuthorize(AppPermissions.Pages_Tenant_Bill_CreateServiceBill)]
        public async Task CreateServiceBill(CreateServiceBillInput input)
        {
            var serviceBill = ObjectMapper.Map<ServiceBill>(input);
            await _serviceBillRepository.InsertAsync(serviceBill);
        }
        //Register Offline
        public async Task CreateInServiceRegister(CreateInServiceRegisterInput input)
        {
            var a = ObjectMapper.Map<UserServiceRegister2>(input);
            await _useSRRepository.InsertAsync(a);
        }

        //////Delete
        //ApartmentBill
        public async Task DeleteApartmentBill(EntityDto input)
        {
            await _apartmentBillRepository.DeleteAsync(input.Id);
        }



        //ServiceBill
        //[AbpAuthorize(AppPermissions.Pages_Tenant_Bill_DeleteServiceBill)]
        public async Task DeleteServiceBill(EntityDto input)
        {
            await _serviceBillRepository.DeleteAsync(input.Id);
        }


        //////GetAllServiceBill
        public async Task<ListResultDto<ServiceBillListDto>> GetCurrentUserServiceBill()
        {
            var currentUser = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());

            var billsByCurrentEmail = _serviceBillRepository.GetAll()
            .WhereIf(
                true,
                p => p.EmailAddress.Equals(currentUser.EmailAddress)
            )
            .OrderBy(p => p.EmailAddress)
            .ToList();

            return new ListResultDto<ServiceBillListDto>(ObjectMapper.Map<List<ServiceBillListDto>>(billsByCurrentEmail));
        }
        //Get list Owner of Apartment for create EAndWBill
        public ListResultDto<UserForBillListDto> GetAllOwnerName()
        {
            var a = _userRepository.GetAll().Where(p => p.ApartmentId != null).ToList();

            return new ListResultDto<UserForBillListDto>(ObjectMapper.Map<List<UserForBillListDto>>(a));
        }
        public async Task<ListResultDto<ApartmentBillListDto>> GetCurrentUserApartmentBill()
        {
            var currentUser = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());

            var billsByCurrentEmail = _apartmentBillRepository.GetAll()
            .WhereIf(
                true,
                p => p.EmailAddress.Equals(currentUser.EmailAddress)
            )
            .OrderBy(p => p.EmailAddress)
            .ToList();

            var mappedList = ObjectMapper.Map<List<ApartmentBillListDto>>(billsByCurrentEmail);

            foreach (var bill in mappedList)
            {
                var apartment = _apartmentRepository.GetAll()
                    .Where(
                    p => p.ApartmentId == currentUser.ApartmentId
                    ).ToList().First();
                
                //var mappedApartment = ObjectMapper.Map<Apartment>(apartment);
                bill.AreaOfApartment = apartment.Area;
                bill.PriceOfApartment = apartment.Price;
            }

            return new ListResultDto<ApartmentBillListDto>(mappedList);
        }







        //////EditBill
        //EditApartmentBill
        //[AbpAuthorize(AppPermissions.Pages_Tenant_Bill_EditApartmentBill)]
        public async Task EditApartmentBill(EditApartmentBill input)
        {
            var a = await _apartmentBillRepository.GetAsync(input.Id);
            a.State = input.State;
            a.BillName = input.BillName;
            a.DatePayment = input.PaymentTerm;
            a.Reason = input.Reason;
            await _apartmentBillRepository.UpdateAsync(a);
        }


        //Edit ServiceBill
        //[AbpAuthorize(AppPermissions.Pages_Tenant_Bill_EditServiceBill)]

        public async Task EditServiceBill(EditServiceBillInput input)
        {
            var serviceBill = await _serviceBillRepository.GetAsync(input.Id);
            serviceBill.BillID = input.BillID;
            serviceBill.BillName = input.BillName;
            serviceBill.ServiceID = input.ServiceID;
            serviceBill.EmailAddress = input.EmailAddress;
            serviceBill.OwnerName = input.OwnerName;
            serviceBill.CreateDay = input.CreateDay;
            serviceBill.CreateName = input.CreateName;
            serviceBill.Cycle = input.Cycle;
            serviceBill.ServiceName = input.ServiceName;
            serviceBill.StartDay = input.StartDay;
            serviceBill.EndDay = input.EndDay;
            serviceBill.PaymentTerm = input.PaymentTerm;
            serviceBill.Note = input.Note;
            serviceBill.State = input.State;
            serviceBill.Price = input.Price;
            serviceBill.Picture = input.Picture;
            serviceBill.DatePayment = input.DatePayment;
            serviceBill.Reason = input.Reason;
            await _serviceBillRepository.UpdateAsync(serviceBill);
        }

        /////Get 
        //Get list ApartmentService for Create bill
        public ListResultDto<ApartmentServiceForBillListDto> GetAllServiceName()
        {
            var a = _apartmentServiceRepository.GetAll().ToList();

            return new ListResultDto<ApartmentServiceForBillListDto>(ObjectMapper.Map<List<ApartmentServiceForBillListDto>>(a));
        }

        //Get list User for Create bill
        public ListResultDto<UserForBillListDto> GetAllUserName()
        {
            var a = _userRepository.GetAll().ToList();

            return new ListResultDto<UserForBillListDto>(ObjectMapper.Map<List<UserForBillListDto>>(a));
        }
        //Get BillingInfo
        public async Task<BillingInformationOutput> GetBillingInformationForView(BillingInformationInput input)
        {
            var b = await _billingInformationRepository.GetAsync(input.Id);
            return ObjectMapper.Map<BillingInformationOutput>(b);
        }
        //Get CreateName
        public async Task<User> GetCreaterNameAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());

            return user;
        }

        //Get Alltype Bills
        public async Task<ListAllBillsDto> GetAllTypeBills()
        {
            var listApartmentBillsInSQL = _apartmentBillRepository.GetAll();
            var listSerBillsInSQL = _serviceBillRepository.GetAll();

            var dbListSerBills = await listSerBillsInSQL.ToListAsync();
            var dbListApartmentBills = await listApartmentBillsInSQL.ToListAsync();

            var listSER = new List<ServiceBillListDto>();
            var listAPA = new List<ApartmentBillListDto>();

            foreach (var e in dbListApartmentBills)
            {
                var res = new ApartmentBillListDto();

                res.Id = e.Id;
                res.BillID = e.BillID;
                res.BillType = e.BillType;
                res.BillName = e.BillName;
                res.EmailAddress = e.EmailAddress;
                res.OwnerName = e.OwnerName;
                res.ApartmentID = e.ApartmentID;
                res.StartDay = e.StartDay;
                res.EndDay = e.EndDay;
                res.OldIndex = e.OldIndex;
                res.NewIndex = e.NewIndex;
                res.PaymentTerm = e.PaymentTerm;
                res.State = e.State;
                res.Price = e.Price;
                res.CreaterName = e.CreaterName;
                res.InvoicePeriod = e.InvoicePeriod;
                res.CreateDay = e.CreateDay;
                res.Picture = e.Picture;
                listAPA.Add(res);
            }

            foreach (var s in dbListSerBills)
            {
                var res = new ServiceBillListDto();
                res.Id = s.Id;
                res.BillID = s.BillID;
                res.ServiceID = s.ServiceID != null ? s.ServiceID : "no";
                res.BillName = s.BillName;
                res.EmailAddress = s.EmailAddress;
                res.OwnerName = s.OwnerName;
                res.CreateDay = s.CreateDay;
                res.CreateName = s.CreateName != null ? s.CreateName : "no";
                res.ServiceName = s.ServiceName;
                res.StartDay = s.StartDay;
                res.EndDay = s.EndDay;
                res.PaymentTerm = s.PaymentTerm;
                res.Note = s.Note != null ? s.Note : "no";
                res.State = s.State != null ? s.State : "no";
                res.Price = s.Price;
                res.Picture = s.Picture;
                listSER.Add(res);
            }

            return new ListAllBillsDto()
            {
                apartmentBillList = listAPA,
                serviceBillList = listSER
            };
        }

        public async Task<long> GetNumberOfServiceBillToday()
        {
            DateTime today = DateTime.Today;
            var serviceBills = await _serviceBillRepository.GetAll()
                .Where(
                p => p.CreationTime.Year == today.Year
                )
                .Where(
                p => p.CreationTime.Month == today.Month
                )
                .Where(
                p => p.CreationTime.Day == today.Day
                )
            .ToListAsync();

            return serviceBills.Count;
        }

        
        public async Task<long> GetNumberOfApartmentBillToday()
        {
            DateTime today = DateTime.Today;
            var apartmentBills = await _apartmentBillRepository.GetAll()
                .Where( 
                p => p.CreationTime.Year == today.Year
                )
                .Where(
                p => p.CreationTime.Month == today.Month
                )
                .Where(
                p => p.CreationTime.Day == today.Day
                )
            .ToListAsync();

            return apartmentBills.Count;
        }
        public async Task<long> GetNumberOfBillToday(DateTime input)
        {
            var s = await _serviceBillRepository.GetAll()
                .Where(
                p => p.CreationTime.Year == input.Year
                )
                .Where(
                p => p.CreationTime.Month == input.Month
                )
                .Where(
                p => p.CreationTime.Day == input.Day
                )
            .ToListAsync();

            var a = await _apartmentBillRepository.GetAll()
                .Where(
                p => p.CreationTime.Year == input.Year
                )
                .Where(
                p => p.CreationTime.Month == input.Month
                )
                .Where(
                p => p.CreationTime.Day == input.Day
                )
            .ToListAsync();

            return (a.Count + s.Count);
        }
        
        //Cập nhật trạng thái nhiều hóa đơn
        //Chưa thanh toán -> Đã thanh toán (Căn hộ)
        [HttpPut]
        public async Task MakeApartmentBillWaiting(NeedUpdateBillInput input)
        {
            foreach (var item in input.List)
            {
                var a = await _apartmentBillRepository.FirstOrDefaultAsync(s => s.Id == item.Id);
                if (a == null)
                {
                    throw new UserFriendlyException("Không tồn tại dịch vụ " + item.Id);
                }

                // Cập nhật status
                if (a.State == "Chưa thanh toán")
                {
                    a.State = "Đã thanh toán";
                    await _apartmentBillRepository.UpdateAsync(a);
                }
            }
        }
        //Dịch vụ: Chưa -> Đã 
        [HttpPut]
        public async Task MakeServiceBillWaiting(NeedUpdateBillInput input)
        {
            foreach (var item in input.List)
            {
                var a = await _serviceBillRepository.FirstOrDefaultAsync(s => s.Id == item.Id);
                if (a == null)
                {
                    throw new UserFriendlyException("Không tồn tại dịch vụ " + item.Id);
                }

                // Cập nhật status
                if (a.State == "Chưa thanh toán")
                {
                    a.State = "Đã thanh toán";
                    await _serviceBillRepository.UpdateAsync(a);
                }
            }
        }
        //Căn hộ: Chờ xác nhận -> Đã thanh toán
        [HttpPut]
        public async Task MakeApartmentBillToDo(NeedUpdateBillInput input)
        {
            foreach (var item in input.List)
            {
                var a = await _apartmentBillRepository.FirstOrDefaultAsync(s => s.Id == item.Id);
                if (a == null)
                {
                    throw new UserFriendlyException("Không tồn tại dịch vụ " + item.Id);
                }

                // Cập nhật status
                if (a.State == "Chờ xác nhận")
                {
                    a.State = "Đã thanh toán";
                    await _apartmentBillRepository.UpdateAsync(a);
                }
            }
        }
        //Dịch vụ: Chờ -> Đã thanh toán 
        [HttpPut]
        public async Task MakeServiceBillToDo(NeedUpdateBillInput input)
        {
            foreach (var item in input.List)
            {
                var a = await _serviceBillRepository.FirstOrDefaultAsync(s => s.Id == item.Id);
                if (a == null)
                {
                    throw new UserFriendlyException("Không tồn tại dịch vụ " + item.Id);
                }

                // Cập nhật status
                if (a.State == "Chờ xác nhận")
                {
                    a.State = "Đã thanh toán";
                    await _serviceBillRepository.UpdateAsync(a);
                }
            }
        }

        public async Task<double> GetTotalUnpaid()
        {
            var currentUser = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            var result = 0D;

            var apartmentBills = _apartmentBillRepository.GetAll()
            .WhereIf(
                true,
                p => p.EmailAddress.Equals(currentUser.EmailAddress)
            )
            .Where(
                p => p.State == "Chưa thanh toán"
            )
            .Where(
                p => p.CreationTime.Month == DateTime.Today.Month
            )
            .OrderBy(p => p.EmailAddress)
            .ToList();
            
            foreach(var apartmentBill in apartmentBills )
            {
                result += apartmentBill.Price;
            }

            var serviceBills = _serviceBillRepository.GetAll()
            .WhereIf(
                true,
                p => p.EmailAddress.Equals(currentUser.EmailAddress)
            )
            .Where(
                p => p.State == "Chưa thanh toán"
            )
            .Where(
                p => p.CreationTime.Month == DateTime.Today.Month
            )
            .OrderBy(p => p.EmailAddress)
            .ToList();

            foreach (var serviceBill in serviceBills)
            {
                result += serviceBill.Price;
            }

            return result;                
        }
    }
}
