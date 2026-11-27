using System.ComponentModel.DataAnnotations;

namespace RC.ViewModels
{
    public class CarVM
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "ماركة السيارة مطلوبة")]
        [StringLength(50)]
        [Display(Name = "الماركة")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "موديل السيارة مطلوب")]
        [StringLength(80)]
        [Display(Name = "الموديل")]
        public string ModelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الوصف مطلوب")]
        [StringLength(1000)]
        [Display(Name = "وصف السيارة")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(1, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        [Display(Name = "السعر")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        [StringLength(120)]
        [Display(Name = "الموقع الحالي")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "صورة السيارة")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }
        public string? ExistingPublicId { get; set; }
    }
}