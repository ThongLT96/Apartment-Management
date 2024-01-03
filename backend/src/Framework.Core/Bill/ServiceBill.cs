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
    [Table("AbpServiceBill")]
    public class ServiceBill : FullAuditedEntity
    {
        public const int MaxBillNameLength = 100;
        public const int MaxBillIDLength = 15;
        public const int MaxEmailAddressLength = 50;
        public const int MaxOwnerNameLength = 50;
        [Required]
        [MaxLength(MaxBillIDLength)]
        public virtual string BillID { get; set; }

        public virtual string ServiceID { get; set; }

        [Required]
        [MaxLength(MaxBillNameLength)]
        public virtual string BillName { get; set; }

        [Required]
        [MaxLength(MaxEmailAddressLength)]
        public virtual string EmailAddress { get; set; }

        [Required]
        [MaxLength(MaxOwnerNameLength)]
        public virtual string OwnerName { get; set; }

        public virtual DateTime CreateDay { get; set; }

        public virtual string CreateName { get; set; }

        [Required]
        public virtual string ServiceName { get; set; }

        public virtual string Cycle { get; set; }

        [Required]
        public virtual DateTime StartDay { get; set; }

        [Required]
        public virtual DateTime EndDay { get; set; }

        [Required]
        public virtual DateTime PaymentTerm { get; set; }

        public virtual string Note { get; set; }
        public virtual string State { get; set; }
        public virtual string Picture { get; set; }

        public virtual long Price { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual DateTime DatePayment { get; set; }
        public virtual string Reason { get; set; }

    }
}
