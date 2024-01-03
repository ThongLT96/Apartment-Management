using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Bill.Dto
{
    public class EditApartmentBill
    {
        public int Id { get; set; }
        public DateTime PaymentTerm { get; set; }
        public string BillName { get; set; }
        public string State { get; set; }
        public string Reason { get; set; }

    }
    public class GetApartmentBillForEditInput
    {
        public int Id { get; set; }
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
        public long Price { get; set; }
        public string CreaterName { get; set; }
        public DateTime InvoicePeriod { get; set; }
        public DateTime CreateDay { get; set; }
        public DateTime DatePayment { get; set; }

    }
    public class GetApartmentBillForEditOutput
    {
        public int Id { get; set; }
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
        public long Price { get; set; }
        public string CreaterName { get; set; }
        public DateTime InvoicePeriod { get; set; }
        public DateTime CreateDay { get; set; }
        public DateTime DatePayment { get; set; }

    }
}
