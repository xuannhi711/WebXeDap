using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = USER_ROLES.ADMIN)]
    public class KhuyenmaiController : Controller
    {
        private readonly IKhuyenmaiRepository _khuyenmaiRepository;

        public KhuyenmaiController(IKhuyenmaiRepository khuyenmaiRepository)
        {
            _khuyenmaiRepository = khuyenmaiRepository;
        }

   
        public async Task<IActionResult> Index()
        {
            var list = await _khuyenmaiRepository.GetAllAsync();
            return View(list);
        }


        public async Task<IActionResult> Details(string id)
        {
            var km = await _khuyenmaiRepository.GetByIdAsync(id);
            if (km == null) return NotFound();

            return View(km);
        }

     
        public IActionResult Create()
        {
            return View();
        }

 
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

        public async Task<IActionResult> Edit(string id)
        {
            var km = await _khuyenmaiRepository.GetByIdAsync(id);
            if (km == null) return NotFound();

            return View(km);
        }

      
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


        public async Task<IActionResult> Delete(string id)
        {
            var km = await _khuyenmaiRepository.GetByIdAsync(id);
            if (km == null) return NotFound();

            return View(km);
        }


        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string MaKM)
        {
            await _khuyenmaiRepository.DeleteAsync(MaKM);
            return RedirectToAction(nameof(Index));
        }
    }

}
