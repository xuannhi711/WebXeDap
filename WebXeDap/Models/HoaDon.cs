using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebXeDap.Models
{
	public class Hoadon
	{
		[Key]
		public int MaHD { get; set; }
		public string UserId { get; set; }
		public DateTime NgayLapHD { get; set; } = DateTime.Now;
		public decimal TongTien { get; set; }
		public string DiaChiGiaoHang { get; set; }
		public string GhiChu { get; set; }
		public string? TrangThai { get; set; }
		public ICollection<Chitiethoadon>? Chitiethoadons { get; set; }

		[ForeignKey("UserId")]
		[ValidateNever]
		public ApplicationUser? ApplicationUser { get; set; }
	}
}
