using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebXeDap.Models
{
	public class Chitiethoadon
	{
		[Key]
		public int Id { get; set; }
		public int MaHD { get; set; }

		[ForeignKey("MaHD")]
		public Hoadon? Hoadon { get; set; }
		public string? Mausac { get; set; }
		public string MaSP { get; set; }

		[ForeignKey("MaSP")]
		public Sanpham? Sanpham { get; set; }
		public int SoLuong { get; set; }
		public decimal DonGia { get; set; }
	}
}
