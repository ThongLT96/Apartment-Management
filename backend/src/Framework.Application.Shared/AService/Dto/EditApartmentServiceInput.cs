using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.AService.Dto
{
    public class EditApartmentServiceInput
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }

        public string Describe { get; set; }
      
        public long ServiceCharge { get; set; }

        public string Cycle { get; set; }

        public string TypeService { get; set; }

        public string ResponsibleUnit { get; set; }
        public DateTime RequestSendDate { get; set; }
        public string Status { get; set; }
        public string URLPicture { get; set; }
        public string Unit { get; set; }
    }
    public class GetApartmentServiceForEditInput
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }

        public string Describe { get; set; }

        public long ServiceCharge { get; set; }

        public string Cycle { get; set; }

        public string TypeService { get; set; }

        public string ResponsibleUnit { get; set; }
        public DateTime RequestSendDate { get; set; }
        public string Status { get; set; }
        public string URLPicture { get; set; }
        public string Unit { get; set; }

    }
    public class GetApartmentServiceForEditOutput
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }

        public string Describe { get; set; }

        public long ServiceCharge { get; set; }

        public string Cycle { get; set; }

        public string TypeService { get; set; }

        public string ResponsibleUnit { get; set; }
        public DateTime RequestSendDate { get; set; }
        public string Status { get; set; }
        public string URLPicture { get; set; }
        public string Unit { get; set; }

    }
}
