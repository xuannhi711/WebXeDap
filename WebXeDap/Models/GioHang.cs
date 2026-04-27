using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebXeDap.Models
{
	public class Giohang
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[Required]
		public string MaSP { get; set; }

		[ForeignKey("MaSP")]
		public virtual Sanpham Sanpham { get; set; }

		public int SoLuong { get; set; }
		public string Mau { get; set; }
		public string HinhAnh { get; set; }
		public decimal DonGia { get; set; }

		public DateTime NgayThem { get; set; } = DateTime.Now;
	}
}
