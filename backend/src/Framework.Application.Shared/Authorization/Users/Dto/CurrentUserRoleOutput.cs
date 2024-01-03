using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Authorization.Users.Dto
{
    public class CurrentUserRoleOutput
    {

        public int RoleId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
}
