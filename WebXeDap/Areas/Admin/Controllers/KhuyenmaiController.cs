using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class KhuyenmaiController : Controller
    {
        private readonly IKhuyenmaiRepository _khuyenmaiRepository;

        public KhuyenmaiController(IKhuyenmaiRepository khuyenmaiRepository)
        {
            _khuyenmaiRepository = khuyenmaiRepository;
        }

        // Index
        public async Task<IActionResult> Index()
        {
            var list = await _khuyenmaiRepository.GetAllAsync();
            return View(list);
        }

        // Details
        public async Task<IActionResult> Details(string id)
        {
            var km = await _khuyenmaiRepository.GetByIdAsync(id);
            if (km == null) return NotFound();

            return View(km);
        }

        // Create GET
        public IActionResult Create()
        {
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khuyenmai khuyenmai)
        {
            if (ModelState.IsValid)
            {
                await _khuyenmaiRepository.AddAsync(khuyenmai);
                return RedirectToAction(nameof(Index));
            }
            return View(khuyenmai);
        }

        // Edit GET
        public async Task<IActionResult> Edit(string id)
        {
            var km = await _khuyenmaiRepository.GetByIdAsync(id);
            if (km == null) return NotFound();

            return View(km);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Khuyenmai khuyenmai)
        {
            if (id != khuyenmai.MaKM) return NotFound();

            if (ModelState.IsValid)
            {
                await _khuyenmaiRepository.UpdateAsync(khuyenmai);
                return RedirectToAction(nameof(Index));
            }
            return View(khuyenmai);
        }

        // Delete GET
        public async Task<IActionResult> Delete(string id)
        {
            var km = await _khuyenmaiRepository.GetByIdAsync(id);
            if (km == null) return NotFound();

            return View(km);
        }

        // Delete POST
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string MaKM)
        {
            await _khuyenmaiRepository.DeleteAsync(MaKM);
            return RedirectToAction(nameof(Index));
        }
    }

}
