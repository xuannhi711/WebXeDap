using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HoadonController : Controller
    {
        private readonly IHoaDonRepository _hoaDonRepo;

        public HoadonController(IHoaDonRepository hoaDonRepo)
        {
            _hoaDonRepo = hoaDonRepo;
        }

        public async Task<IActionResult> Index()
        {
            var hoadons = await _hoaDonRepo.GetAllAsync();
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

            // Chỉ lấy các hóa đơn chưa "Đã thanh toán" hoặc "Đã duyệt"
            var donHangMoi = hoadons.Where(hd => !hd.TrangThai.Contains("Đã thanh toán") && !hd.TrangThai.Contains("Đã duyệt"));

            return View(donHangMoi);
        }

        // Xác nhận đơn hàng
        public async Task<IActionResult> XacNhan(int id)
        {
            var hoadon = await _hoaDonRepo.GetByIdAsync(id);
            if (hoadon == null) return NotFound();

            // Kiểm tra nếu chưa có "Đã thanh toán" hoặc "Đã duyệt" thì cập nhật
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
    }
}
