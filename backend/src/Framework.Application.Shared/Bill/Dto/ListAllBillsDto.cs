using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Bill.Dto
{
    public class ListAllBillsDto
    {
        public List<ApartmentBillListDto> apartmentBillList { get; set; }
        public List<ServiceBillListDto> serviceBillList { get; set; }
    }
}
