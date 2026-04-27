using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options) { }

	public DbSet<Loai> Loais { get; set; }
	public DbSet<Nhacungcap> Nhacungcaps { get; set; }
	public DbSet<Khuyenmai> Khuyenmais { get; set; }
	public DbSet<Sanpham> Sanphams { get; set; }
	public DbSet<Giohang> Giohangs { get; set; }
	public DbSet<Hoadon> Hoadon { get; set; }
	public DbSet<Chitiethoadon> Chitiethoadons { get; set; }
	public DbSet<Baohanh> Baohanhs { get; set; }
	public DbSet<Anh> Anhs { get; set; }
	public DbSet<tintuc> tintuc { get; set; }
	public DbSet<Mau> Maus { get; set; }
	public DbSet<Feedback> Feedback { get; set; }
}
