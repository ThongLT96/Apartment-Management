using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;


namespace Framework.AService.Dto
{
    public class GetAllApartmentServicesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }
    }
}
