using System.ComponentModel.DataAnnotations;

namespace portal.Models
{
    public class BirchFormModel
    {
        [Required(ErrorMessage = "không được bỏ trống")]
        [Range(1, 9999, ErrorMessage = "không được vượt quá [1, 9999]")]
        public int? B { get; set; }
        [Required(ErrorMessage = "không được bỏ trống")]
        [Range(0, 99999, ErrorMessage = "không được vượt quá [1, 99999]")]
        public float? T { get; set; }
        [Required(ErrorMessage = "không được bỏ trống")]
        [Range(1, 9999, ErrorMessage = "không được vượt quá [1, 9999]")]
        public int? L { get; set; }
        public int? K { get; set; }
        [Required(ErrorMessage = "Không tìm thấy file dữ liệu nào")]
        public IFormFile DataCollection { get; set; }
    }
}
