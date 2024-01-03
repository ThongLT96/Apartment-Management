using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Authorization.Users.Dto
{
    public class GetUserByEmailInput
    {
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EmaillAddress { get; set; }
    }
}
