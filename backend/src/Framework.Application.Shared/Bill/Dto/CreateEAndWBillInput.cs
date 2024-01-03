using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Framework.Bill.Dto
{
    public class CreateEAndWBillInput
    {
        [Required]
        [MaxLength(BillConsts.MaxBillIDLength)]
        public string BillID { get; set; }
        public string BillType { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxBillNameLength)]
        public string BillName { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxOwnerNameLength)]
        public string OwnerName { get; set; }

        [Required]
        public string ApartmentID { get; set; }

        [Required]
        public DateTime StartDay { get; set; }

        [Required]
        public DateTime EndDay { get; set; }

        [Required]
        public long OldIndex { get; set; }

        [Required]
        public long NewIndex { get; set; }

        [Required]
        public DateTime PaymentTerm { get; set; }

        public string State { get; set; }
        public string Price { get; set; }
    }
}
