using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace Framework.Bill.Dto
{
    public class CreateServiceBillInput
    {
        [Required]
        [MaxLength(BillConsts.MaxBillIDLength)]
        public string BillID { get; set; }

        public string ServiceID { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxBillNameLength)]
        public string BillName { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(BillConsts.MaxOwnerNameLength)]
        public string OwnerName { get; set; }

        public DateTime CreateDay { get; set; }

        public string CreateName { get; set; }

        [Required]
        public string ServiceName { get; set; }

        public string Cycle { get; set; }

        [Required]
        public DateTime StartDay { get; set; }

        [Required]
        public DateTime EndDay { get; set; }

        [Required]
        public DateTime PaymentTerm { get; set; }

        public string Note { get; set; }
        public string State { get; set; }
        public string Picture { get; set; }
        public long Price { get; set; }
    }
}
