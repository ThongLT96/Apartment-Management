using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Bill.Dto
{
    public class UserForBillListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string ApartmentId { get; set; }
    }
}
