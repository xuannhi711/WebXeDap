using System.ComponentModel.DataAnnotations;

namespace WebXeDap.Models
{
    public class Baohanh
    {
        [Key]
        public int MaBaoHanh { get; set; }

        public string MaHD { get; set; }
        public string MaSP { get; set; }
        public Chitiethoadon chitiethoadon { get; set; }

        public int ThoiHanBaoHanh { get; set; }
        public string DieuKienBaoHanh { get; set; }
        public DateTime NgayTaoBaoHanh { get; set; } = DateTime.Now;
    }

}
