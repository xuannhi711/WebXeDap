using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebXeDap.Models
{
	public class Sanpham
	{
		[Key]
		public string MaSP { get; set; }
		public string TenSP { get; set; }
		public decimal GiaMua { get; set; }
		public decimal GiaBan { get; set; }

		public int SoLuongTon { get; set; }
		public string MaLoai { get; set; }

		[ForeignKey("MaLoai")]
		public Loai? Loai { get; set; }
		public int TGBH { get; set; }
		public string MaNCC { get; set; }

		[ForeignKey("MaNCC")]
		public Nhacungcap? Nhacungcap { get; set; }

		public string MaKM { get; set; }

		[ForeignKey("MaKM")]
		public Khuyenmai? Khuyenmai { get; set; }
		public List<Anh>? Anhs { get; set; }
		public string mota { get; set; }
		public List<Mau>? Mau { get; set; }

		[NotMapped]
		public decimal GiaKhuyenMai =>
			Khuyenmai != null ? GiaBan * (1 - Khuyenmai.GiamGia / 100) : GiaBan;
	}
}
