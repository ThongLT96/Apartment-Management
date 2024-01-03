using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace Framework.AService.Dto
{
    public class NeedDeleteApartmentServicesInput
    {
        public List<ApartmentServiceInput> List { get; set; }
    }

    public class ApartmentServiceInput
    {
        public virtual int? Id { get; set; }
        //public string ServiceName { get; set; }
        //public string Status { get; set; }
    }
}
