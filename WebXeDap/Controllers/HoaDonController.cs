using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Repositories;

namespace WebXeDap.Controllers
{
	[Authorize]
	public class HoaDonController : Controller
	{
		private readonly IHoaDonRepository _hoaDonRepo;
		private readonly IBaohanhRepository _baohanhRepo;

		public HoaDonController(IHoaDonRepository hoaDonRepo, IBaohanhRepository baohanhRepo)
		{
			_hoaDonRepo = hoaDonRepo;
			_baohanhRepo = baohanhRepo;
		}

		public async Task<IActionResult> Index()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var hoadons = await _hoaDonRepo.GetByUserIdAsync(userId);
			return View(hoadons);
		}

		public async Task<IActionResult> Details(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var hoadon = await _hoaDonRepo.GetByIdAsync(id);

			if (hoadon == null || hoadon.UserId != userId)
			{
				return NotFound();
			}

			return View(hoadon);
		}

		public async Task<IActionResult> BaoHanh(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var hoadon = await _hoaDonRepo.GetByIdAsync(id);

			if (hoadon == null || hoadon.UserId != userId)
			{
				return NotFound();
			}

			var baohanhs = await _baohanhRepo.GetByUserIdAsync(userId);

			return View(baohanhs);
		}

		public async Task<IActionResult> DanhSachBaoHanh()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var baohanhs = await _baohanhRepo.GetByUserIdAsync(userId);
			return View(baohanhs);
		}
	}
}
