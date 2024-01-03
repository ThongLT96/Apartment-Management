using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Bill.Dto
{
    public class ListRoleUser
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string TypeAccount { get; set; }
        public string EmailAddress { get; set; }
        public string ApartmentId { get; set; }
        public string PhoneNumber { get; set; }
        public string IDNumber { get; set; }
        public string ProfileAvatar { get; set; }
    }
}
