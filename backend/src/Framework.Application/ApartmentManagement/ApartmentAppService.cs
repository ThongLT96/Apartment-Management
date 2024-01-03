using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Framework.ApartmentManagement.Dto;
using Framework.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ApartmentManagement
{
    //[AbpAuthorize(AppPermissions.Pages_Administration_apartment)]
    public class ApartmentAppService : FrameworkAppServiceBase, IApartmentAppService
    {
        private readonly IRepository<Apartment> _apartmentRepository;

        public ApartmentAppService(IRepository<Apartment> apartmentRepository)
        {
            _apartmentRepository = apartmentRepository;
        }

        [AbpAllowAnonymous]
        public async Task<ListResultDto<GetApartmentOutputDto>> GetAllApartments()
        {
            var a = _apartmentRepository.GetAll();
            return new ListResultDto<GetApartmentOutputDto>(ObjectMapper.Map<List<GetApartmentOutputDto>>(a));
        }

        public async Task CreateApartment(AddApartmentInputDto input)
        {
            var apartment = ObjectMapper.Map<Apartment>(input);
            apartment.StatusDeleted = 1;
            await _apartmentRepository.InsertAsync(apartment);
        }

        public async Task<GetApartmentOutputDto> GetApartmentById(string id)
        {
            var apartment = _apartmentRepository.FirstOrDefault(x => x.ApartmentId == id);
            if (apartment == null)
            {
                return null;
            }
            else
            {
                return ObjectMapper.Map<GetApartmentOutputDto>(apartment);
            }
        }

        public async Task UpdateApartment(UpdateApartmentInputDto input)
        {
            var apartment = _apartmentRepository.FirstOrDefault(x => x.ApartmentId == input.ApartmentId);
            apartment.OwnerId = input.OwnerId;
            apartment.OwnerName = input.OwnerName;
            apartment.AmountOfPeople = input.AmountOfPeople;
            apartment.AmountOfRooms = input.AmountOfRooms;
            apartment.BuildingId = input.BuildingId;
            apartment.Floor = input.Floor;
            apartment.Area = input.Area;
            apartment.Price = input.Price;
            apartment.Status = input.Status;
            apartment.StatusDeleted = input.StatusDeleted;

            await _apartmentRepository.UpdateAsync(apartment);
        }

        public async Task DeleteApartment(string id)
        {
            var apartment = _apartmentRepository.FirstOrDefault(x => x.ApartmentId == id);
            
            apartment.StatusDeleted = 0;
            apartment.IsDeleted = true;
            await _apartmentRepository.UpdateAsync(apartment);
        }
    }
}