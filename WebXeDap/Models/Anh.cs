using System.ComponentModel.DataAnnotations.Schema;

namespace WebXeDap.Models
{
    public class Anh
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string MaSP { get; set; }
        [ForeignKey("MaSP")]
        public Sanpham? sanpham { get; set; }
    }
 
}
