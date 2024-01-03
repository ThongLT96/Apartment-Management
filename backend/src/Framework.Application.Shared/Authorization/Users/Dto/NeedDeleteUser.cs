using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Authorization.Users.Dto
{
    public class NeedDeleteUser
    {
        public List<UserInput> List { get; set; }
    }
    public class UserInput
    {
        public virtual int? Id { get; set; }
    }
}
