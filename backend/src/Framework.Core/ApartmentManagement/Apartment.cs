using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ApartmentManagement
{
    [Table("AbpApartments")]
    public class Apartment : FullAuditedEntity
    {
        [Required]
        public virtual string ApartmentId { get; set; }

        public virtual int OwnerId { get; set; }


        public virtual string OwnerName { get; set; }
        
        public virtual int AmountOfPeople { get; set; }

        public virtual int AmountOfRooms { get; set; }

        public virtual string BuildingId { get; set; }

        public virtual int Floor { get; set; }

        public virtual float Area { get; set; }

        public virtual float Price { get; set; }

        public virtual string Status { get; set; }

        public virtual int StatusDeleted { get; set; }
    }
}
