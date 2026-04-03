using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebXeDap.Models
{
    public class Baohanh
    {
        [Key]
        public int MaBaoHanh { get; set; }
        public int MaHD { get; set; }
        public string MaSP { get; set; }
        public int ThoiHanBaoHanh { get; set; }
        public string DieuKienBaoHanh { get; set; }
        public DateTime NgayTaoBaoHanh { get; set; }
        public DateTime NgayHetHanBaoHanh { get; set; }

        [ForeignKey("MaHD")]
        public Hoadon Hoadon { get; set; }
    }
}

