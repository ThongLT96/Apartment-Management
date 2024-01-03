using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ApartmentManagement.Dto
{
    public class GetApartmentOutputDto
    {
        public string ApartmentId { get; set; }

        public int OwnerId { get; set; }

        public string OwnerName { get; set; }

        public int AmountOfPeople { get; set; }

        public int AmountOfRooms { get; set; }

        public string BuildingId { get; set; }

        public int Floor { get; set; }

        public float Area { get; set; }

        public float Price { get; set; }

        public string Status { get; set; }
        
        public int StatusDeleted { get; set; }
    }
}
