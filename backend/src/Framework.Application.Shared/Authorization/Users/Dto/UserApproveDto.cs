using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Authorization.Users.Dto
{
    public class UserApproveDto
    {
        // User INFO
        public long? UserId { get; set; }
        public string EmailAddress { get; set; }
        public string ApartmentId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        // Approval INFO
        public bool IsApproved { get; set; }
        public List<string> RejectReasons { get; set; }
    }
}
