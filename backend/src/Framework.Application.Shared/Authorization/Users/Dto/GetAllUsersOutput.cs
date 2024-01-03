using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Authorization.Users.Dto
{
    public class GetAllUsersOutput
    {
        [Key][Required]
        public Int64 Id { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public string ApartmentId { get; set; }
    }
}
