using System.ComponentModel.DataAnnotations;

namespace TestWeb.Models
{
    public class Apartment
    {
        [Key]
        public string ApartmentId { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int AmountOfPeople { get; set; }
        public int AmountOfRooms { get; set; }
        public string BuildingId { get; set; }
        public int Floor { get; set; }
        public float Area { get; set; }
        public float Price { get; set; }
        public string Status { get; set; }
    }
}
