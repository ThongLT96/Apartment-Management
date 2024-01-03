using System;
using System.Collections.Generic;
using System.Text;
using Framework.AService;
using Framework.AService.Dto;
using Abp.Application.Services.Dto;

namespace Framework.Bill.Dto
{
    public class ApartmentServiceForBillListDto /*: ApartmentServiceListDto*/
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }

        //public string Describe { get; set; }

        public long ServiceCharge { get; set; }

        public string Cycle { get; set; }

        //public string TypeService { get; set; }

        //public string ResponsibleUnit { get; set; }
    }
}
