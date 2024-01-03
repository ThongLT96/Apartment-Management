using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Bill.Dto
{
    public class EAndWBillListDto
    {
        public string BillID { get; set; }
        public string BillType { get; set; }

        
        public string BillName { get; set; }

       
        public string EmailAddress { get; set; }

        
        public string OwnerName { get; set; }

      
        public string ApartmentID { get; set; }

       
        public DateTime StartDay { get; set; }

       
        public DateTime EndDay { get; set; }

        
        public long OldIndex { get; set; }

  
        public long NewIndex { get; set; }

      
        public DateTime PaymentTerm { get; set; }

        public string State { get; set; }
        public string Price { get; set; }
    }
}
