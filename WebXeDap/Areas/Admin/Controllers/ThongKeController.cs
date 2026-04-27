using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;

namespace WebXeDap.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = USER_ROLES.ADMIN)]
	public class ThongKeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ThongKeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(
			DateTime? startDate,
			DateTime? endDate,
			string reportType
		)
		{
			List<DailyRevenueData> revenueData = new List<DailyRevenueData>();

			if (startDate.HasValue && endDate.HasValue && !string.IsNullOrEmpty(reportType))
			{
				revenueData = await GetRevenueData(startDate.Value, endDate.Value, reportType);
			}

			return View(revenueData);
		}

		public async Task<IActionResult> thang()
		{
			var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			var endDate = startDate.AddMonths(1).AddDays(-1);

			var revenueData = await GetRevenueData(startDate, endDate, "daily");

			return View(revenueData);
		}

		private async Task<List<DailyRevenueData>> GetRevenueData(
			DateTime startDate,
			DateTime endDate,
			string reportType
		)
		{
			IQueryable<DailyRevenueData> query;

			switch (reportType)
			{
				case "monthly":
					query =
						from hd in _context.Hoadon
						join cthd in _context.Chitiethoadons on hd.MaHD equals cthd.MaHD
						where hd.NgayLapHD >= startDate && hd.NgayLapHD <= endDate
						group cthd by new { hd.NgayLapHD.Year, hd.NgayLapHD.Month } into g
						select new DailyRevenueData
						{
							Date = new DateTime(g.Key.Year, g.Key.Month, 1),
							TotalQuantity = g.Sum(x => x.SoLuong),
							TotalRevenue = g.Sum(x => x.SoLuong * x.DonGia),
						};
					break;

				case "yearly":
					query =
						from hd in _context.Hoadon
						join cthd in _context.Chitiethoadons on hd.MaHD equals cthd.MaHD
						where hd.NgayLapHD >= startDate && hd.NgayLapHD <= endDate
						group cthd by hd.NgayLapHD.Year into g
						select new DailyRevenueData
						{
							Date = new DateTime(g.Key, 1, 1),
							TotalQuantity = g.Sum(x => x.SoLuong),
							TotalRevenue = g.Sum(x => x.SoLuong * x.DonGia),
						};
					break;

				case "daily":
				default:
					query =
						from hd in _context.Hoadon
						join cthd in _context.Chitiethoadons on hd.MaHD equals cthd.MaHD
						where hd.NgayLapHD >= startDate && hd.NgayLapHD <= endDate
						group cthd by hd.NgayLapHD.Date into g
						select new DailyRevenueData
						{
							Date = g.Key,
							TotalQuantity = g.Sum(x => x.SoLuong),
							TotalRevenue = g.Sum(x => x.SoLuong * x.DonGia),
						};
					break;
			}

			return await query.ToListAsync();
		}

		public class DailyRevenueData
		{
			public DateTime Date { get; set; }
			public int TotalQuantity { get; set; }
			public decimal TotalRevenue { get; set; }
		}
	}
}
