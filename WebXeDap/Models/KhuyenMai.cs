using System.ComponentModel.DataAnnotations;

namespace WebXeDap.Models
{
    public class Khuyenmai
    {
        [Key]
        public string MaKM { get; set; }

        public string TenKhuyenMai { get; set; }
        public string MoTa { get; set; }
        public decimal GiamGia { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public List<Sanpham>? Sanpham { get; set; }
    }

}
