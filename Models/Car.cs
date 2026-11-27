using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RC.Models
{
    public class Car
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "ماركة السيارة مطلوبة")]
        [StringLength(50)]
        [Display(Name = "الماركة")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "موديل السيارة مطلوب")]
        [StringLength(80)]
        [Display(Name = "الموديل")]
        public string ModelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الوصف مطلوب")]
        [StringLength(1000, ErrorMessage = "الوصف لا يتجاوز 1000 حرف")]
        [Display(Name = "وصف السيارة")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(1, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        [Display(Name = "السعر (بالدولار)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        [StringLength(120)]
        [Display(Name = "الموقع الحالي")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "رابط الصورة")]
        public string ImageUrl { get; set; } = string.Empty;

        public string CloudinaryPublicId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsAvailable { get; set; } = true;
    }
}