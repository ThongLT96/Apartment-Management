using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Framework.UserServiceRegister.Dto;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Framework.UserServiceRegister
{
    public interface IUserServiceRegister : IApplicationService
    {
        Task<ListResultDto<UserRegisterDto>> GetServiceRegistered();
        Task<int> RegisterService(RegisterServiceInput input);
        Task UnregisterService(int id);

        Task<ListResultDto<UserRegisterDto>> GetAllServiceRegister();

    }
}
