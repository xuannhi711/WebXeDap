using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models;

namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ThongKeDoanhThu()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Doanh thu từng ngày trong tháng hiện tại
            var doanhThuTheoNgay = _context.Hoadon
                .Where(h => h.NgayLapHD >= firstDayOfMonth && h.NgayLapHD <= lastDayOfMonth && h.TrangThai == "Đã thanh toán")
                .GroupBy(h => h.NgayLapHD.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    TongTien = g.Sum(h => h.TongTien)
                })
                .OrderBy(g => g.Ngay)
                .ToList();

            // Tạo danh sách ngày & doanh thu
            List<string> ngayList = new List<string>();
            List<decimal> doanhThuList = new List<decimal>();

            for (var day = firstDayOfMonth; day <= lastDayOfMonth; day = day.AddDays(1))
            {
                ngayList.Add(day.ToString("dd/MM"));
                var doanhThuNgay = doanhThuTheoNgay.FirstOrDefault(d => d.Ngay == day)?.TongTien ?? 0;
                doanhThuList.Add(doanhThuNgay);
            }

            ViewBag.NgayList = ngayList;
            ViewBag.DoanhThuList = doanhThuList;

            return View();
        }
    }
}
