﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AspNetZeroCore.Validation;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Extensions;
using Framework.Authorization.Users;
using Framework.Security;

namespace Framework.Web.Models.Account
{
    public class RegisterViewModel : IValidatableObject
    {
        [Required]
        [StringLength(User.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(User.MaxSurnameLength)]
        public string Surname { get; set; }

        [StringLength(User.MaxNameLength + User.MaxSurnameLength)]
        public string FullName { get; set; }


        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [MaxLength(UserConsts.MaxGenderLength)]
        public string Gender { get; set; }

        [MaxLength(UserConsts.MaxIDNumberLength)]
        public string IDNumber { get; set; }

        public DateTime BirthDate { get; set; }

        public string BuildingId { get; set; }
        public string ApartmentId { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool IsExternalLogin { get; set; }

        public string ExternalLoginAuthSchema { get; set; }

        public string ReturnUrl { get; set; }

        public string SingleSignIn { get; set; }

        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserName.IsNullOrEmpty())
            {
                if (!UserName.Equals(EmailAddress, StringComparison.OrdinalIgnoreCase) && ValidationHelper.IsEmail(UserName))
                {
                    yield return new ValidationResult("Username cannot be an email address unless it's same with your email address !");
                }
            }
        }
    }
}