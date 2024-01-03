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
    [Table("AbpEAndWBill")]
    public class EAndWBill : FullAuditedEntity
    {
        public const int MaxBillNameLength = 100;
        public const int MaxBillIDLength = 15;
        public const int MaxEmailAddressLength = 50;
        public const int MaxOwnerNameLength = 50;
        [Required]
        [MaxLength(MaxBillIDLength)]
        public virtual string BillID { get; set; }
        public virtual string BillType { get; set; }

        [Required]
        [MaxLength(MaxBillNameLength)]
        public virtual string BillName { get; set; }

        [Required]
        [MaxLength(MaxEmailAddressLength)]
        public virtual string EmailAddress { get; set; }

        [Required]
        [MaxLength(MaxOwnerNameLength)]
        public virtual string OwnerName { get; set; }

        [Required]
        public virtual string ApartmentID { get; set; }

        [Required]
        public virtual DateTime StartDay { get; set; }

        [Required]
        public virtual DateTime EndDay { get; set; }

        [Required]
        public virtual long OldIndex { get; set; }

        [Required]
        public virtual long NewIndex { get; set; }

        [Required]
        public virtual DateTime PaymentTerm { get; set; }

        public virtual string State { get; set; }
        public virtual string Price { get; set; }
    }
}
