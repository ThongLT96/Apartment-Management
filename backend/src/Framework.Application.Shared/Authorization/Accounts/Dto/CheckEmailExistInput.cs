using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Authorization.Accounts.Dto
{
    public class CheckEmailExistInput
    {
        [EmailAddress(ErrorMessage = "Không phải email!")]
        public string EmailAddress { get; set; }
    }
}
