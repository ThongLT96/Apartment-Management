using Abp.Application.Services.Dto;
using Framework.Bill.Dto;
using Framework.UserServiceRegister.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Authorization.Users.Dto
{
    public class UserDetailsOutput
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Key][Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string IDNumber { get; set; }
        public string ApartmentId { get; set; }

        public ListResultDto<UserRegisterDto> UserServices { get; set; }

        public ListResultDto<ApartmentBillListDto> UserApartmentBills { get; set; }
        public ListResultDto<ServiceBillListDto> UserServiceBills { get; set; }
    }
}
