using System;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;

namespace Framework.Authorization.Users.Profile.Dto
{
    public class CurrentUserProfileEditDto
    {
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(UserConsts.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(UserConsts.MaxGenderLength)]
        public string Gender { get; set; }

        [Required]
        [MaxLength(UserConsts.MaxIDNumberLength)]
        public string IDNumber { get; set; }

        public string ApartmentId { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
        public virtual bool IsPhoneNumberConfirmed { get; set; }

        public string OTP { get; set; }

        public string Timezone { get; set; }

        public string QrCodeSetupImageUrl { get; set; }

        public bool IsGoogleAuthenticatorEnabled { get; set; }
    }
}