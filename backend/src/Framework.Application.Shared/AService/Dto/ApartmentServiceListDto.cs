using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace Framework.AService.Dto
{
    public class ApartmentServiceListDto : FullAuditedEntityDto
    {
        public string ServiceName { get; set; }

        public string Describe { get; set; }

        public long ServiceCharge { get; set; }

        public string Cycle { get; set; }

        public string TypeService { get; set; }

        public string ResponsibleUnit { get; set; }
        public DateTime RequestSendDate { get; set; }
        public string Status { get; set; }
        public string URLPicture { get; set; }
        public string Unit { get; set; }
    }
}
