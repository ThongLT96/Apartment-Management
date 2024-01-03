using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Framework.Authorization.Users;
using Framework.Bill;
using Framework.ServiceRegister;
using Framework.UserServiceRegister.Dto;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.UserServiceRegister
{
    public class UserServiceRegisterAppService : FrameworkAppServiceBase, IUserServiceRegister
    {
        private readonly IRepository<UserServiceRegister2> _serviceRegisterRepository;
        public UserManager _UserManager { get; set; }
        public IAbpSession _AbpSession { get; set; }
        public UserServiceRegisterAppService(
            IRepository<UserServiceRegister2> serviceRegisterRepository,
             UserManager userManager)
        {
            _serviceRegisterRepository = serviceRegisterRepository;
            _UserManager = userManager;
            _AbpSession = NullAbpSession.Instance;
        }

        public async Task<ListResultDto<UserRegisterDto>> GetAllServiceRegister()
        {
            var registers = await _serviceRegisterRepository.GetAllListAsync();
            return new ListResultDto<UserRegisterDto>(ObjectMapper.Map<List<UserRegisterDto>>(registers));
        }

        public async Task<ListResultDto<UserRegisterDto>> GetServiceRegistered()
        {
            var currentUser = await _UserManager.FindByIdAsync(_AbpSession.GetUserId().ToString());

            var service = _serviceRegisterRepository
            .GetAll()   
            .WhereIf(
                true,
                p => p.EmailAddress.Equals(currentUser.EmailAddress)
            )
            .OrderBy(p => p.EmailAddress)
            .ToList();

            return new ListResultDto<UserRegisterDto>(ObjectMapper.Map<List<UserRegisterDto>>(service));
        }

        public async Task<int> RegisterService(RegisterServiceInput input)
        {
            var service = new RegisterServiceInput
            {
                BillID = input.BillID,
                BillName = input.BillName,
                ServiceID = input.ServiceID,
                ServiceName = input.ServiceName,
                OwnerName = input.OwnerName,
                PhoneNumber = input.PhoneNumber,
                Cycle = input.Cycle,
                State = input.State,
                startDate = input.startDate,
                endDate = input.endDate,
                EmailAddress = input.EmailAddress,
                Price = input.Price,
                Note = input.Note,
                TypeService = input.TypeService,
                UrlPicture= input.UrlPicture,
            }; 
            var serviceRegister = ObjectMapper.Map<UserServiceRegister2>(service);
            return await _serviceRegisterRepository.InsertAndGetIdAsync(serviceRegister);
        }

        public async Task UnregisterService(int id)
        {
            await _serviceRegisterRepository.DeleteAsync(id);
        }

        public async Task<bool> CheckRegisteredService(string serviceId)
        {
            var currentUser = await _UserManager.FindByIdAsync(_AbpSession.GetUserId().ToString());

            var list = await _serviceRegisterRepository.GetAll()
                .Where(
                 p => p.EmailAddress == currentUser.EmailAddress
                 )
                .Where(
                 p => p.ServiceID == serviceId
                 )
                .Where(
                 p => p.IsDeleted == false
                 ).ToListAsync();
            return true ? list.Count > 0 : false;
        }
    }
}
