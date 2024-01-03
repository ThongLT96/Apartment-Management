using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.AService.Dto
{
    public class CreateApartmentServiceInput
    {
        [Required]
        [MaxLength(ApartmentServiceConsts.MaxServiceNameLength)]
        public string ServiceName { get; set; }

        public string Describe { get; set; }

        [Required]
        //[MaxLength(ApartmentServiceConsts.MaxServiceChargeLength)]
        public long ServiceCharge { get; set; }

        [MaxLength(ApartmentServiceConsts.MaxCycleLength)]
        public string Cycle { get; set; }

        [Required]
        [MaxLength(ApartmentServiceConsts.MaxTypeServiceLength)]
        public string TypeService { get; set; }

        [MaxLength(ApartmentServiceConsts.MaxResponsibleUnitLength)]
        public string ResponsibleUnit { get; set; }
        public string Status { get; set; }
        public DateTime RequestSendDate { get; set; }
        public string URLPicture { get; set; }
        public string Unit { get; set; }
    }
}
