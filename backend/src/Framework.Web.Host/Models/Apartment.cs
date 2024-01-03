//using System.ComponentModel.DataAnnotations;

//namespace Framework.Web.Models
//{
//    public class Apartment
//    {
//        [Key]
//        [Required]
//        public string ApartmentId { get; set; }
//        public string OwnerId { get; set; }
//        public string OwnerName { get; set; }
//        [Range(minimum: 0, maximum: 20, ErrorMessage = "Số người không được là số âm")]
//        public int AmountOfPeople { get; set; }
//        [Range(minimum: 1, maximum: 3, ErrorMessage = "Số lượng phòng là từ 1 đến 3")]
//        public int AmountOfRooms { get; set; }
//        public string BuildingId { get; set; }
//        [Range(minimum: 0, maximum: 11, ErrorMessage = "Số tầng là một số từ 0 đến 11")]
//        public int Floor { get; set; }
//        [Range(minimum: 25, maximum: 9999999, ErrorMessage = "Diện tích phải là số lớn hơn 25")]
//        public float Area { get; set; }
//        [Range(minimum: 0, maximum: 9999999, ErrorMessage = "Giá không được là số âm")]
//        public float Price { get; set; }
//        public string Status { get; set; }
//    }
//}
