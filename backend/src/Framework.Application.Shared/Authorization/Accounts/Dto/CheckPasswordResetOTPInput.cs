using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace Framework.Authorization.Accounts.Dto
{
    public class CheckPasswordResetOTPInput
    {
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public string OTP { get; set; }
    }
}
