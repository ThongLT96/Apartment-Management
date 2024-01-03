using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Authorization.Users.Dto
{
    public class GetUserByEmailOutput
    {
        public Int64 Id { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string IDNumber { get; set; }
        public string ApartmentId { get; set; }
        public string AccountType { get; set; }
        public string UserName { get; set; }
        public string ProfileAvatar { get; set; }
    }
}
