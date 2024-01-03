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
    [Table("AbpApartmentBill")]
    public class ApartmentBill : FullAuditedEntity
    {
        public virtual string BillID { get; set; }
        public virtual string BillType { get; set; }

        public virtual string BillName { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string OwnerName { get; set; }

        public virtual string ApartmentID { get; set; }

        public virtual DateTime StartDay { get; set; }

        public virtual DateTime EndDay { get; set; }

        public virtual long OldIndex { get; set; }

        public virtual long NewIndex { get; set; }

        public virtual DateTime PaymentTerm { get; set; } //Han thanh toan

        public virtual string State { get; set; }
        public virtual long Price { get; set; }
        public virtual string CreaterName { get; set; }
        public virtual DateTime InvoicePeriod { get; set; }
        public virtual DateTime CreateDay { get; set; }
        public virtual DateTime DatePayment { get; set; }
        public virtual string Reason { get; set; }
        public virtual string Picture { get; set; }

    }
}
