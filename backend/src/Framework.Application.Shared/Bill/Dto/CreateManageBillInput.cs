using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Framework.Bill.Dto
{
    public class CreateManageBillInput
    {
        [Required]
        [MaxLength(BillConsts.MaxBillIDLength)]
        public string BillID { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxBillNameLength)]
        public string BillName { get; set; }

        public string BillType { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxOwnerNameLength)]
        public string OwnerName { get; set; }

        public string ApartmentID { get; set; }

        public string CreateName { get; set; }

        public DateTime InvoicePeriod { get; set; }

        [Required]
        public DateTime PaymentTerm { get; set; }
        public string State { get; set; }

        public long Price { get; set; }
    }
}
