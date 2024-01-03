using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Framework.AService.Dto;
using System.Threading.Tasks;
using Framework.Dto;
using Framework.Admin.Dtos;

namespace Framework.AService
{
    public interface IApartmentServiceAppService : IApplicationService
    {
        ListResultDto<ApartmentServiceListDto> GetApartmentService(GetApartmentServiceInput input);
        Task CreateApartmentService(CreateApartmentServiceInput input);
        Task DeleteApartmentService(EntityDto input);
        Task<GetApartmentServiceForEditOutput> GetApartmentServiceForEdit(GetApartmentServiceForEditInput input);

        Task EditApartmentService(EditApartmentServiceInput input);

        Task MakeApartmentServicesWaiting(NeedDeleteApartmentServicesInput input);
        Task MakeApartmentServicesDeleted(NeedDeleteApartmentServicesInput input);

    }
}
