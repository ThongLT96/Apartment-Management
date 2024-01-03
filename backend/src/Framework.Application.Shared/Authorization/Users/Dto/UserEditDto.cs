using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Extensions;

namespace Framework.Authorization.Users.Dto
{
    //Mapped to/from User in CustomDtoMapper
    public class UserEditDto : IPassivable, IValidatableObject
    {
        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }

        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength + AbpUserBase.MaxNameLength)]
        public string FullName { get; set; }

        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(UserConsts.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(UserConsts.MaxGenderLength)]
        public string Gender { get; set; }

        [MaxLength(UserConsts.MaxIDNumberLength)]
        public string IDNumber { get; set; }

        public string BuildingId { get; set; }

        public string ApartmentId { get; set; }

        public DateTime BirthDate { get; set; }

        public string ProfileAvatar { get; set; }

        // Not used "Required" attribute since empty value is used to 'not change password'
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public virtual bool IsTwoFactorEnabled { get; set; }

        public virtual bool IsLockoutEnabled { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!PhoneNumber.IsNullOrEmpty())
            {
                foreach (char c in PhoneNumber)
                {
                    if (c < '0' || c > '9')
                    {
                        yield return new ValidationResult("Số điện thoại chỉ chứa kỳ tự số");
                        break;
                    }
                }
            }
        }
    }
}