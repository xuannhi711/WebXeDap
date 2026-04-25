using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = USER_ROLES.ADMIN)]
    public class LoaiController : Controller
    {
        private readonly ILoaiRepository _loaiRepository;

        public LoaiController(ILoaiRepository loaiRepository)
        {
            _loaiRepository = loaiRepository;
        }
        public async Task<IActionResult> Index()
        {
            var loai = await _loaiRepository.GetAllAsync();
            return View(loai);
        }

        public async Task<IActionResult> Display(string  id)
        {

            var loai = await _loaiRepository.GetByIdAsync(id);
            if (loai == null)
            {
                return NotFound();
            }

            return View(loai);
        }
        public async Task<IActionResult> Add()
        {


            return View();
        }

        // Xử lý thêm sản phẩm mới 
        [HttpPost]
        public async Task<IActionResult> Add(Loai product)
        {
            if (ModelState.IsValid)
            {

                await _loaiRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        public async Task<IActionResult> Update(string  id)
        {
            var category = await _loaiRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, Loai category)
        {
            if (id != category.MaLoai)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _loaiRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public async Task<IActionResult> Delete(string  id)
        {

            var loai = await _loaiRepository.GetByIdAsync(id);
            if (loai == null)
            {
                return NotFound();

   }
            return View(loai);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(string  id)
        {
            var loai = await _loaiRepository.GetByIdAsync(id);
            if (loai != null)
            {
                _loaiRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
