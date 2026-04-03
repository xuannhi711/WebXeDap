using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models;


namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class TintucController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TintucController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tintucs = _context.tintuc.ToList();
            return View(tintucs);
        }

        public IActionResult Add()
        {
            ViewBag.AllImages = _context.tintuc.Where(t => t.tieude == "poster").ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile Image)
        {
            if (Image != null && Image.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                var tin = new tintuc
                {
                    hinhanh = "/images/" + uniqueFileName,
                    tieude = "poster",
                    noidung = "poster",
                    ngaytao = DateTime.Now
                };

                _context.tintuc.Add(tin);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Add));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var tin = await _context.tintuc.FindAsync(id);
            if (tin == null || tin.tieude != "poster") return NotFound();

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", tin.hinhanh.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.tintuc.Remove(tin);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Add));
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(tintuc tin, IFormFile Image)
        {
            if (Image != null && Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
                tin.hinhanh = "/images/" + fileName;
            }

            tin.ngaytao = DateTime.Now;
            _context.tintuc.Add(tin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Danhsach));
        }

        // Edit
        public async Task<IActionResult> Edit(int id)
        {
            var tin = await _context.tintuc.FindAsync(id);
            if (tin == null) return NotFound();
            return View(tin);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, tintuc updatedTin, IFormFile? Image)
        {
            var tin = await _context.tintuc.FindAsync(id);
            if (tin == null) return NotFound();

            tin.tieude = updatedTin.tieude;
            tin.noidung = updatedTin.noidung;

            if (Image != null && Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                // Xóa ảnh cũ
                if (!string.IsNullOrEmpty(tin.hinhanh))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", tin.hinhanh.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                tin.hinhanh = "/images/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Danhsach));
        }
        public IActionResult Danhsach()
        {
            var list = _context.tintuc
                        .Where(t => t.tieude != "poster")
                        .OrderByDescending(t => t.ngaytao)
                        .ToList();
            return View(list);
        }
        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            var tin = await _context.tintuc.FindAsync(id);
            if (tin == null) return NotFound();
            return View(tin);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tin = await _context.tintuc.FindAsync(id);
            if (tin == null) return NotFound();

            if (!string.IsNullOrEmpty(tin.hinhanh))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", tin.hinhanh.TrimStart('/'));
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            _context.tintuc.Remove(tin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Danhsach));
        }
    }
}
