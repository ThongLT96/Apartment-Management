using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Framework.AService
{
    [Table("AbpApartmentServices")]
    public class ApartmentService : FullAuditedEntity
    {
        public const int MaxServiceNameLength = 100;
        public const int MaxResponsibleUnitLength = 100;
        public const int MaxTypeServiceLength = 15;
        public const int MaxCycleLength = 20;
        [Required]
        [MaxLength(MaxServiceNameLength)]
        public virtual string ServiceName { get; set; }

        public virtual string Describe { get; set; }

        [Required]
        public virtual long ServiceCharge { get; set; }

        [MaxLength(MaxCycleLength)]
        public virtual string Cycle { get; set; }

        [Required]
        [MaxLength(MaxTypeServiceLength)]
        public virtual string TypeService { get; set; }

        [MaxLength(MaxResponsibleUnitLength)]
        public virtual string ResponsibleUnit { get; set; }
        public virtual string Status { get; set; }
        public virtual DateTime RequestSendDate { get; set; }
        public virtual string URLPicture { get; set; }
        public virtual string Unit { get; set; }
    }
}
