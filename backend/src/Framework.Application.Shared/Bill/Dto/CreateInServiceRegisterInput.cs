﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Bill.Dto
{
    public class CreateInServiceRegisterInput
    {
        public string BillID { get; set; }

        public string ServiceID { get; set; }

        
        public string BillName { get; set; }

       
        public string EmailAddress { get; set; }

        
        public string OwnerName { get; set; }

        public DateTime CreateDay { get; set; }

        
        public string ServiceName { get; set; }
        
        public string Cycle { get; set; }

        
        public DateTime StartDay { get; set; }

       
        public DateTime EndDay { get; set; }

        
        public DateTime PaymentTerm { get; set; }

        
        public string PhoneNumber { get; set; }

        public string State { get; set; }
        public string Note { get; set; }

        public long Price { get; set; }
    }
}
