using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Authorization.Accounts.Dto
{
    public class ConfirmEmailAddressOTPInput
    {
        public string EmailAddress { get; set; }
        public string OTP { get; set; }
    }
}
