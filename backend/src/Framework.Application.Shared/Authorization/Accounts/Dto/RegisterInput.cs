using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Extensions;
using Framework.Authorization.Users;
using Framework.Validation;

namespace Framework.Authorization.Accounts.Dto
{
    public class RegisterInput : IValidatableObject
    {
        public string OTP { get; set; }

        [StringLength(AbpUserBase.MaxNameLength, ErrorMessage = "Tên của bạn quá dài!")]
        public string Name { get; set; }

        [StringLength(AbpUserBase.MaxSurnameLength, ErrorMessage = "Họ của bạn quá dài!")]
        public string Surname { get; set; }

        [StringLength(AbpUserBase.MaxNameLength + AbpUserBase.MaxSurnameLength, ErrorMessage = "Tên của bạn quá dài!")]
        public string FullName { get; set; }

        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(UserConsts.MaxPhoneNumberLength, ErrorMessage = "Số điện thoại quá dài")]
        public string PhoneNumber { get; set; }

        [MaxLength(UserConsts.MaxGenderLength)]
        public string Gender { get; set; }

        public string IDNumber { get; set; }

        public DateTime BirthDate { get; set; }

        public string BuildingId { get; set; }

        public string ApartmentId { get; set; }
        
        public bool AgreeToTermsAndConditions { get; set; }

        [StringLength(UserConsts.MaxPlainPasswordLength, MinimumLength = UserConsts.MinPlainPasswordLength,
            ErrorMessage = "Mật khẩu phải có 6 đến 20 ký tự")]
        [DisableAuditing]
        public string Password { get; set; }

        [DisableAuditing]
        public string CaptchaResponse { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserName.IsNullOrEmpty())
            {
                if (!UserName.Equals(EmailAddress, StringComparison.OrdinalIgnoreCase) && ValidationHelper.IsEmail(UserName))
                {
                    yield return new ValidationResult("Username cannot be an email address unless it's same with your email address !");
                }
            }

            if (FullName.Trim().Equals(string.Empty) || (EmailAddress.Trim().Equals(string.Empty) && PhoneNumber.Trim().Equals(string.Empty)) || IDNumber.Trim().Equals(string.Empty) || Password.Equals(string.Empty)
                || BuildingId.Trim().Equals(string.Empty) || ApartmentId.Trim().Equals(string.Empty))
            {
                yield return new ValidationResult("Vui lòng nhập đầy thông tin");
            }

            if (Password.Contains(" "))
            {
                yield return new ValidationResult("Mật khẩu không được chứa khoảng trống");
            }

            if (!(IDNumber.Length == 9 || IDNumber.Length == 12))
            {
                yield return new ValidationResult("Vui lòng nhập 9 số đối với CMND hoặc 12 số đối với CCCD");
            }
            else
            {
                foreach (char c in IDNumber)
                {
                    if (c < '0' || c > '9')
                    {
                        yield return new ValidationResult("CMND hoặc CCCD không hợp lệ");
                        break;
                    }
                }
            }

            if (!PhoneNumber.IsNullOrEmpty())
            {
                foreach (char c in PhoneNumber)
                {
                    if (c < '0' || c > '9')
                    {
                        yield return new ValidationResult("Số điện thoại hơi kì");
                        break;
                    }
                }
            }

            if (PhoneNumber.IsNullOrEmpty() && !EmailAddress.Contains("@")) { 
                yield return new ValidationResult("Email không hợp lệ");
            }

            if (EmailAddress.Trim().Contains(" "))
            { 
                yield return new ValidationResult("Email không được chứa khoảng trắng!");
            }
        }
    }
}