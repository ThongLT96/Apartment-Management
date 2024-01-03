using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.UserServiceRegister.Dto
{
    public class RegisterServiceInput
    {
        public string BillID;

        public string BillName;

        public string ServiceID;

        public string ServiceName;

        public string OwnerName;

        public string PhoneNumber;

        public string EmailAddress;

        public string Cycle;

        public DateTime startDate;

        public DateTime endDate;

        public long Price;

        public string Note;

        public string State;

        public string TypeService;

        public string UrlPicture;
    }
}
