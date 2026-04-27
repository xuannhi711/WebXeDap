using System.ComponentModel.DataAnnotations.Schema;

namespace WebXeDap.Models
{
	public class Mau
	{
		public int Id { get; set; }
		public string TenMau { get; set; }
		public string MaSP { get; set; }

		[ForeignKey("MaSP")]
		public Sanpham? sanpham { get; set; }
	}
}
