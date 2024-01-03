using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Framework.Bill
{
    [Table("AbpBillingInformation")]
    public class BillingInformation : FullAuditedEntity
    {
        [Required]
        public virtual string AccountName { get; set; }

        [Required]
        public virtual string AccountNumber { get; set; }

        [Required]
        public virtual string Bank { get; set; }

        [Required]
        public virtual string Content { get; set; }

        [Required]
        public virtual DateTime DayTrading { get; set; }
    }
}
