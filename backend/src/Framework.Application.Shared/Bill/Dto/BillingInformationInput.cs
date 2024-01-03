using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Bill.Dto
{
    public class BillingInformationInput
    {
        public int Id { get; set; }
    }
    public class BillingInformationOutput
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Bank { get; set; }
        public string Content { get; set; }
        public string DayTrading { get; set; }
    }
}
