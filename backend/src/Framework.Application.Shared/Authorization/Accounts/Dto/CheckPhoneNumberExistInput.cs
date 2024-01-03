using Abp.UI;
using Framework.Authorization.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Framework.Authorization.Accounts.Dto
{
    public class CheckPhoneNumberExistInput : IValidatableObject
    {
        private const string errorMessage_KhongHopLe = "Số điện thoại không hợp lệ";

        [Phone(ErrorMessage = errorMessage_KhongHopLe)]
        [MaxLength(UserConsts.MaxPhoneNumberLength, ErrorMessage = errorMessage_KhongHopLe)]
        public string PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PhoneNumber.Contains(" "))
            {
                yield return new ValidationResult("Vui lòng không nhập khoảng trống!");
            }
        }
    }
}
