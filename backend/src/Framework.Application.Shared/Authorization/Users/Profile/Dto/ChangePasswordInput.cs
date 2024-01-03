using System.ComponentModel.DataAnnotations;
using Abp.Auditing;

namespace Framework.Authorization.Users.Profile.Dto
{
    public class ChangePasswordInput
    {
        [Required]
        [DisableAuditing]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(UserConsts.MaxPlainPasswordLength, MinimumLength = UserConsts.MinPlainPasswordLength)]
        [DisableAuditing]
        public string NewPassword { get; set; }
    }
}