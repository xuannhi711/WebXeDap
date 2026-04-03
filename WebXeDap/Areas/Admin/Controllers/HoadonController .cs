using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HoadonController : Controller
    {
        private readonly IHoaDonRepository _hoaDonRepo;
        private readonly IBaohanhRepository _baohanhRepo;
        private readonly INguoiDungRepository _nguoiDungRepo;

        public HoadonController(IHoaDonRepository hoaDonRepo, IBaohanhRepository baohanhRepo, INguoiDungRepository nguoiDungRepo)
        {
            _hoaDonRepo = hoaDonRepo;
            _baohanhRepo = baohanhRepo;
            _nguoiDungRepo = nguoiDungRepo;
        }


        public async Task<IActionResult> Index(string searchMaHD, DateTime? startDate, DateTime? endDate)
        {
            ViewData["CurrentFilterMaHD"] = searchMaHD;
            ViewData["CurrentStartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["CurrentEndDate"] = endDate?.ToString("yyyy-MM-dd");

            var hoadons = await _hoaDonRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(searchMaHD))
            {
                hoadons = hoadons.Where(h => h.MaHD.ToString().Contains(searchMaHD));
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                hoadons = hoadons.Where(h => h.NgayLapHD >= startDate.Value && h.NgayLapHD <= endDate.Value);
            }

            var newOrderCount = hoadons.Count(h => h.TrangThai == "Chờ xác nhận");
            ViewData["NewOrderCount"] = newOrderCount;

            return View(hoadons);
        }

        public async Task<IActionResult> Details(int id)
        {
            var hoadon = await _hoaDonRepo.GetByIdAsync(id);
            if (hoadon == null) return NotFound();
            return View(hoadon);
        }
        public async Task<IActionResult> DonHangMoi()
        {
            var hoadons = await _hoaDonRepo.GetAllAsync();


            var donHangMoi = hoadons.Where(hd => !hd.TrangThai.Contains("Đã thanh toán") && !hd.TrangThai.Contains("Đã duyệt"));

            return View(donHangMoi);
        }


        public async Task<IActionResult> XacNhan(int id)
        {
            var hoadon = await _hoaDonRepo.GetByIdAsync(id);
            if (hoadon == null) return NotFound();

 
            if (!hoadon.TrangThai.Contains("Đã thanh toán") && !hoadon.TrangThai.Contains("Đã duyệt"))
            {
                hoadon.TrangThai = "Đã duyệt";
                await _hoaDonRepo.UpdateAsync(hoadon);
                TempData["Success"] = "Hóa đơn đã được duyệt!";
            }
            else
            {
                TempData["Warning"] = "Hóa đơn đã được thanh toán hoặc duyệt trước đó!";
            }

            return RedirectToAction(nameof(DonHangMoi));
        }
        public async Task<IActionResult> BaoHanhList(string searchString, string searchType)
        {
            IEnumerable<Baohanh> baohanhs;

            if (!string.IsNullOrEmpty(searchString))
            {
                if (searchType == "MaBaoHanh")
                {
                    baohanhs = await _baohanhRepo.GetByMaBaoHanhAsync(searchString);
                }
                else if (searchType == "UserId")
                {
                    baohanhs = await _baohanhRepo.GetByUserIdAsync(searchString);
                }
                else if (searchType == "Email")
                {
                    var user = await _nguoiDungRepo.GetByEmailAsync(searchString);
                    if (user != null)
                    {
                        baohanhs = await _baohanhRepo.GetByUserIdAsync(user.Id);
                    }
                    else
                    {
                        baohanhs = Enumerable.Empty<Baohanh>();
                    }
                }
                else
                {
                    baohanhs = await _baohanhRepo.GetAllAsync();
                }
            }
            else
            {
                baohanhs = await _baohanhRepo.GetAllAsync();
            }

            return View(baohanhs);
        }
    }
}
