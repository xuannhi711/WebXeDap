using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = USER_ROLES.ADMIN)]
    public class SanPhamController : Controller
    {
        private readonly ISanphamRepository _sanphamRepository;
        private readonly ApplicationDbContext _context;

        public SanPhamController(ISanphamRepository sanphamRepositoryt, ApplicationDbContext context)
        {
            _sanphamRepository = sanphamRepositoryt; _context = context;
        }

   
        public async Task<IActionResult> Index()
        {
            var sanphams = await _sanphamRepository.GetAllAsync();
            return View(sanphams);
        }

  
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null) return NotFound();

            return View(sanpham);
        }

       
        public IActionResult Add()
        {
            LoadDropdowns();
            return View();
        }

        // Create POST
        [HttpPost]
        public async Task<IActionResult> Add(Sanpham product, List<IFormFile> Images, List<string> SelectedColors)
        {
            if (ModelState.IsValid)
            {
                await _sanphamRepository.AddAsync(product);

                if (SelectedColors != null && SelectedColors.Count > 0)
                {
                    foreach (var color in SelectedColors)
                    {
                        var mau = new Mau
                        {
                            TenMau = color,
                            MaSP = product.MaSP
                        };
                        await _sanphamRepository.AddMauAsync(mau);
                    }
                }

                // B2: Kiểm tra có ảnh không, nếu có thì lưu
                if (Images != null && Images.Count > 0)
                {
                    var imagePaths = await SaveImages(Images); 

                    foreach (var path in imagePaths)
                    {
                        var productImage = new Anh
                        {
                            MaSP = product.MaSP,
                            Url = path
                        };

                        await _sanphamRepository.AddAnhAsync(productImage);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(product);
        }

        private async Task<List<string>> SaveImages(List<IFormFile> images)
        {
            List<string> savedPaths = new List<string>();

            foreach (var image in images)
            {
                if (image != null && image.Length > 0)
                {
                  
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                    // Đường dẫn thư mục wwwroot/images
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    savedPaths.Add("/images/" + uniqueFileName);
                }
            }

            return savedPaths;
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null)
                return NotFound();

            LoadDropdowns();
            return View(sanpham);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Sanpham sanpham, List<string> SelectedColors, List<IFormFile> Images)
        {
            if (id != sanpham.MaSP) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {

                    await _sanphamRepository.UpdateAsync(sanpham);


                    var existingColors = _context.Maus.Where(m => m.MaSP == sanpham.MaSP).ToList();
                    _context.Maus.RemoveRange(existingColors);
                    if (SelectedColors != null && SelectedColors.Count > 0)
                    {
                        foreach (var color in SelectedColors)
                        {
                            var mau = new Mau
                            {
                                TenMau = color,
                                MaSP = sanpham.MaSP
                            };
                            await _sanphamRepository.AddMauAsync(mau);
                        }
                    }

                    if (Images != null && Images.Count > 0)
                    {
                        var imagePaths = await SaveImages(Images);

                        foreach (var path in imagePaths)
                        {
                            var productImage = new Anh
                            {
                                MaSP = sanpham.MaSP,
                                Url = path
                            };

                            await _sanphamRepository.AddAnhAsync(productImage);
                        }
                    }

                    return RedirectToAction(nameof(Edit), new { id = sanpham.MaSP });
                }
                catch (Exception)
                {
                    return NotFound();
                }
            }

            LoadDropdowns();
            return View(sanpham);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.Anhs.FindAsync(id);
            if (image == null)
                return Json(new { success = false, message = "Ảnh không tồn tại" });

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Anhs.Remove(image);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null) return NotFound();

            return View(sanpham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _sanphamRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdowns()
        {
            ViewBag.MaLoai = new SelectList(_context.Loais.ToList(), "MaLoai", "TenLoai");
            ViewBag.MaNCC = new SelectList(_context.Nhacungcaps.ToList(), "MaNCC", "TenNCC");
            ViewBag.MaKM = new SelectList(_context.Khuyenmais.ToList(), "MaKM", "TenKhuyenMai");
        }
    }
}

