using Framework.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Authorization.Accounts.Dto
{
    public class RegisterByEmailInput
    {
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EmailAddress { get; set; }
    }
}
