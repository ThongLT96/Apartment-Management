using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Framework.ApartmentManagement.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ApartmentManagement
{
    public interface IApartmentAppService : IApplicationService
    {
        Task<ListResultDto<GetApartmentOutputDto>> GetAllApartments();
        Task CreateApartment(AddApartmentInputDto input);
        Task<GetApartmentOutputDto> GetApartmentById(string id);
        Task UpdateApartment(UpdateApartmentInputDto input);
        Task DeleteApartment(string id);
    }
}
