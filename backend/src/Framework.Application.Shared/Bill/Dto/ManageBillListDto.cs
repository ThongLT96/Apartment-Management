using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Bill.Dto
{
    public class ManageBillListDto
    {
        public string BillID { get; set; }

        public string BillName { get; set; }

        public string BillType { get; set; }

        public string EmailAddress { get; set; }

        public string OwnerName { get; set; }

        public string ApartmentID { get; set; }

        public string CreateName { get; set; }

        public DateTime InvoicePeriod { get; set; }

        public DateTime PaymentTerm { get; set; }
        public string State { get; set; }

        public long Price { get; set; }
    }
}
