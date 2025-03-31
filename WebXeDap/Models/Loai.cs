using System.ComponentModel.DataAnnotations;

namespace WebXeDap.Models
{
    public class Loai
    {
        [Key]
        public string MaLoai { get; set; }

        public string TenLoai { get; set; }
        public string MoTa { get; set; }
        public List<Sanpham>? Sanpham { get; set; }
    }

}
