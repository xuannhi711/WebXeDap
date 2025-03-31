using System.ComponentModel.DataAnnotations;

namespace WebXeDap.Models
{
    public class Nhacungcap
    {
        [Key]
        public string MaNCC { get; set; }

        public string TenNCC { get; set; }
        public string SDT { get; set; }
        public string DiaChi { get; set; }
        public List<Sanpham>? Sanpham { get; set; }
    }

}
