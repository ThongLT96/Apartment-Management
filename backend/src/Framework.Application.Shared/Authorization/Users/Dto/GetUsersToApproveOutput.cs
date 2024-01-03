using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Authorization.Users.Dto
{
    public class GetUsersToApproveOutput
    {
        public List<UserApproveDto> Users { get; set; }
    }
}
