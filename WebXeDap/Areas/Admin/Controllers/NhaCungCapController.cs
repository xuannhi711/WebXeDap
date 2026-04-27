using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = USER_ROLES.ADMIN)]
	public class NhacungcapController : Controller
	{
		private readonly INhacungcapRepository _nhacungcapRepository;

		public NhacungcapController(INhacungcapRepository nhacungcapRepository)
		{
			_nhacungcapRepository = nhacungcapRepository;
		}

		public async Task<IActionResult> Index()
		{
			var nhacungcaps = await _nhacungcapRepository.GetAllAsync();
			return View(nhacungcaps);
		}

		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(Nhacungcap nhacungcap)
		{
			if (ModelState.IsValid)
			{
				await _nhacungcapRepository.AddAsync(nhacungcap);
				return RedirectToAction(nameof(Index));
			}
			return View(nhacungcap);
		}

		public async Task<IActionResult> Update(string id)
		{
			var ncc = await _nhacungcapRepository.GetByIdAsync(id);
			if (ncc == null)
				return NotFound();

			return View(ncc);
		}

		[HttpPost]
		public async Task<IActionResult> Update(string id, Nhacungcap nhacungcap)
		{
			if (id != nhacungcap.MaNCC)
				return NotFound();

			if (ModelState.IsValid)
			{
				await _nhacungcapRepository.UpdateAsync(nhacungcap);
				return RedirectToAction(nameof(Index));
			}

			return View(nhacungcap);
		}

		public async Task<IActionResult> Delete(string id)
		{
			var ncc = await _nhacungcapRepository.GetByIdAsync(id);
			if (ncc == null)
				return NotFound();

			return View(ncc);
		}

		[HttpPost, ActionName("DeleteConfirmed")]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			await _nhacungcapRepository.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(string id)
		{
			var ncc = await _nhacungcapRepository.GetByIdAsync(id);
			if (ncc == null)
				return NotFound();

			return View(ncc);
		}
	}
}
