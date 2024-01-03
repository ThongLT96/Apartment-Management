using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Framework.Bill.Dto
{
    public class NeedUpdateBillInput
    {
        public List<BillInput> List { get; set; }
    }
    public class BillInput
    {
        public virtual int? Id { get; set; }
    }
}
