using System.ComponentModel.DataAnnotations;

namespace WebXeDap.Models
{
	public class tintuc
	{
		public int id { get; set; }
		public string tieude { get; set; }
		public string noidung { get; set; }
		public string? hinhanh { get; set; }
		public DateTime? ngaytao { get; set; } = DateTime.Now;
	}
}
