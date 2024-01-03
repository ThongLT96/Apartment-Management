using Abp.Auditing;
using Framework.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Authorization.Accounts.Dto
{
    public class ResetPasswordWithPhoneNumberInput
    {
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ, vui lòng chỉ nhập kí tự số")]
        [StringLength(UserConsts.MaxPhoneNumberLength, ErrorMessage = "Số điện thoại quá dài")]
        public string PhoneNumber { get; set; }
        public long UserId { get; set; }

        [StringLength(UserConsts.MaxPlainPasswordLength, MinimumLength = UserConsts.MinPlainPasswordLength,
            ErrorMessage = "Mật khẩu phải có 6 đến 20 ký tự")]
        [DisableAuditing]
        public string Password { get; set; }
    }
}
