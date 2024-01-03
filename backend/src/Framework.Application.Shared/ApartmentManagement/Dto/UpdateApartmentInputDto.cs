using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.ApartmentManagement.Dto
{
    public class UpdateApartmentInputDto
    {
        [Required]
        public string ApartmentId { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required]
        public string OwnerName { get; set; }

        [Required]
        public int AmountOfPeople { get; set; }

        [Required]
        public int AmountOfRooms { get; set; }

        [Required]
        public string BuildingId { get; set; }

        [Required]
        public int Floor { get; set; }

        [Required]
        public float Area { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public int StatusDeleted { get; set; }
    }
}
