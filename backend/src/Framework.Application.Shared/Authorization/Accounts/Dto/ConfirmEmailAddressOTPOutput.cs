using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Authorization.Accounts.Dto
{
    public class ConfirmEmailAddressOTPOutput
    {
        public bool IsCorrect { get; set; }

        public string Message { get; set; }
    }
}
