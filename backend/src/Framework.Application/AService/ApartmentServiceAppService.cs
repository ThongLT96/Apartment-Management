using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.AService.Dto;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Framework.AService;
using Framework.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;

namespace Framework.AService
{
    //[AbpAuthorize(AppPermissions.Pages_Tenant_AService)]
    public class ApartmentServiceAppService : FrameworkAppServiceBase, IApartmentServiceAppService
    {
        private readonly IRepository<ApartmentService> _apartmentServiceRepository;

        public ApartmentServiceAppService(IRepository<ApartmentService> apartmentServiceRepository)
        {
            _apartmentServiceRepository = apartmentServiceRepository;
        }

        public ListResultDto<ApartmentServiceListDto> GetApartmentService(GetApartmentServiceInput input)
        {
            var apartmentService = _apartmentServiceRepository
                .GetAll()
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Status == input.Filter
                )
                .OrderBy(p => p.ServiceName)
                .ThenBy(p => p.TypeService)
                .ToList();

            return new ListResultDto<ApartmentServiceListDto>(ObjectMapper.Map<List<ApartmentServiceListDto>>(apartmentService));
        }

        public ListResultDto<ApartmentServiceListDto> GetApartmentServiceFilterStatusWaiting()
        {
            var apartmentService = _apartmentServiceRepository.GetAll().Where(p => p.Status == "existing" || p.Status == "canceled").ToList();

            return new ListResultDto<ApartmentServiceListDto>(ObjectMapper.Map<List<ApartmentServiceListDto>>(apartmentService));
        }
        public ListResultDto<ApartmentServiceListDto> GetApartmentServiceFilterStatusCanceled()
        {
            var apartmentService = _apartmentServiceRepository.GetAll().Where(p => p.Status == "waiting" || p.Status == "canceled" || p.Status == "deleted").OrderBy(p => p.Status).ToList();

            return new ListResultDto<ApartmentServiceListDto>(ObjectMapper.Map<List<ApartmentServiceListDto>>(apartmentService));
        }
        //Create 
        //[AbpAuthorize(AppPermissions.Pages_Tenant_AService_CreateApartmentService)]
        public async Task CreateApartmentService(CreateApartmentServiceInput input)
        {
            var apartmentService = ObjectMapper.Map<ApartmentService>(input);
            await _apartmentServiceRepository.InsertAsync(apartmentService);
        }
        //Delete
        //[AbpAuthorize(AppPermissions.Pages_Tenant_AService_DeleteApartmentService)]
        public async Task DeleteApartmentService(EntityDto input)
        {
            await _apartmentServiceRepository.DeleteAsync(input.Id);
        }
        public async Task<GetApartmentServiceForEditOutput> GetApartmentServiceForEdit(GetApartmentServiceForEditInput input)
        {
            var apartmentService = await _apartmentServiceRepository.GetAsync(input.Id);
            return ObjectMapper.Map<GetApartmentServiceForEditOutput>(apartmentService);
        }
        //[AbpAuthorize(AppPermissions.Pages_Tenant_AService_EditApartmentService)]
        public async Task EditApartmentService(EditApartmentServiceInput input)
        {
            var apartmentService = await _apartmentServiceRepository.GetAsync(input.Id);
            apartmentService.ServiceName = input.ServiceName;
            apartmentService.Describe = input.Describe;
            apartmentService.ServiceCharge = input.ServiceCharge;
            apartmentService.TypeService = input.TypeService;
            apartmentService.Cycle = input.Cycle;
            apartmentService.ResponsibleUnit = input.ResponsibleUnit;
            apartmentService.RequestSendDate = input.RequestSendDate;
            apartmentService.URLPicture = input.URLPicture;
            apartmentService.Status = input.Status;
            apartmentService.Unit = input.Unit;
            await _apartmentServiceRepository.UpdateAsync(apartmentService);
        }

        // Input: List<ApartmentService> - danh sách các ApartmentService
        // Output: tạm thời không quan trọng, cần thiết thì sẽ trả về string thông báo gì đó
        [HttpPut]
        public async Task MakeApartmentServicesWaiting(NeedDeleteApartmentServicesInput input)
        {
            // Tạm bỏ qua việc kiểm tra tenantId
            // Lấy từ database những dịch vụ có id tương ứng
            foreach (var item in input.List)
            {
                var service = await _apartmentServiceRepository.FirstOrDefaultAsync(s => s.Id == item.Id);
                if (service == null)
                {
                    throw new UserFriendlyException("Không tồn tại dịch vụ " + item.Id);
                }

                // Cập nhật status
                if (service.Status == "existing")
                {
                    service.Status = "waiting";
                    service.RequestSendDate = DateTime.Now;
                    await _apartmentServiceRepository.UpdateAsync(service);

                    // thích kiểm trả kết quả update thì kiểm tra, không thì thôi
                    // ...

                }
            }
        }

        [HttpPut]
        public async Task MakeApartmentServicesDeleted(NeedDeleteApartmentServicesInput input)
        {
            foreach (var item in input.List)
            {
                var service = await _apartmentServiceRepository.FirstOrDefaultAsync(s => s.Id == item.Id);
                if (service == null)
                {
                    throw new UserFriendlyException("Không tồn tại dịch vụ " + item.Id);
                }

                // Cập nhật status
                if (service.Status == "waiting")
                {
                    service.Status = "deleted";
                    service.RequestSendDate = DateTime.Now;
                    await _apartmentServiceRepository.UpdateAsync(service);

                }
            }
        }
        //Waiting -> Canceled
        [HttpPut]
        public async Task MakeApartmentServiceCanceled(NeedDeleteApartmentServicesInput input)
        {
            foreach (var item in input.List)
            {
                var service = await _apartmentServiceRepository.FirstOrDefaultAsync(s => s.Id == item.Id);
                if (service == null)
                {
                    throw new UserFriendlyException("Không tồn tại dịch vụ " + item.Id);
                }

                // Cập nhật status
                if (service.Status == "waiting")
                {
                    service.Status = "canceled";
                    service.RequestSendDate = DateTime.Now;
                    await _apartmentServiceRepository.UpdateAsync(service);

                }
            }
        }

        //Total page
        public async Task<PagedResultDto<GetApartmentServiceForViewDto>> GetAll(GetAllApartmentServicesInput input)
        {

            var filteredApartmentServices = _apartmentServiceRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ServiceName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.TypeService == input.NameFilter);

            var pagedAndFilteredApartmentServices = filteredApartmentServices
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var apartmentServices = from o in pagedAndFilteredApartmentServices
                                    select new
                          {
                              o.TypeService,
                              o.ServiceName,
                              o.ServiceCharge,
                              Id = o.Id
                          };

            var totalCount = await filteredApartmentServices.CountAsync();

            var dbList = await apartmentServices.ToListAsync();
            var results = new List<GetApartmentServiceForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetApartmentServiceForViewDto()
                {
                    ApartmentService = new ApartmentServiceListDto
                    {
                        TypeService = o.TypeService,
                        ServiceName = o.ServiceName,
                        ServiceCharge = o.ServiceCharge,
                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetApartmentServiceForViewDto>(
                totalCount,
                results
            );
        }
        //Return MaxApaermentServiceId
        public async Task<int> GetIdMax()
        {
            int max = 0;
            var a = _apartmentServiceRepository.GetAll().ToList();
            foreach(var i in a)
            {
                if (i.Id > max)
                {
                    max = i.Id;
                }

            }
            return max;
        }
    }

}
