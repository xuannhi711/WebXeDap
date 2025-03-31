using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ISanphamRepository _sanphamRepository;
        private readonly ILoaiRepository _loaiRepository;
        private readonly INhacungcapRepository _nhacungcapRepository;
        private readonly ApplicationDbContext _context;

        public SanPhamController(ISanphamRepository sanphamRepositoryt,INhacungcapRepository nhacungcap, ApplicationDbContext context, ILoaiRepository loaiRepository)
        {   _nhacungcapRepository = nhacungcap;
            _sanphamRepository = sanphamRepositoryt;
            _context = context;
            _loaiRepository = loaiRepository;


        }

        // GET: Sanpham
        public async Task<IActionResult> Index(int? minPrice, int? maxPrice, string? category, string? brand)
        {
            var sanphams = await _sanphamRepository.GetAllAsync();

            if (minPrice.HasValue && maxPrice.HasValue)
            {
                sanphams = sanphams.Where(sp => sp.GiaBan >= minPrice.Value && sp.GiaBan <= maxPrice.Value).ToList();
            }

            if (!string.IsNullOrEmpty(category))
            {
                sanphams = sanphams.Where(sp => sp.MaLoai == category).ToList();
            }

            if (!string.IsNullOrEmpty(brand))
            {
                sanphams = sanphams.Where(sp => sp.MaNCC == brand).ToList();
            }

            // Lấy danh sách tất cả danh mục và nhà cung cấp để hiển thị bộ lọc
            ViewBag.DanhMuc = await _loaiRepository.GetAllAsync();
            ViewBag.Brands = await _nhacungcapRepository.GetAllAsync();

            return View(sanphams);
        }



        // GET: Sanpham/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null) return NotFound();

            return View(sanpham);
        }

        // GET: Sanpham/Create
        public IActionResult Add()
        {
            LoadDropdowns();
            return View();
        }

        // Create POST
        [HttpPost]
        public async Task<IActionResult> Add(Sanpham product, List<IFormFile> Images)
        {
            if (ModelState.IsValid)
            {
                // B1: Lưu sản phẩm vào bảng Product trước
                await _sanphamRepository.AddAsync(product);

                // B2: Kiểm tra có ảnh không, nếu có thì lưu
                if (Images != null && Images.Count > 0)
                {
                    var imagePaths = await SaveImages(Images); // Lưu vào thư mục và trả về danh sách đường dẫn

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
                    // Tạo tên file duy nhất
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                    // Đường dẫn thư mục wwwroot/images
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                    // Tạo file và copy dữ liệu
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Thêm đường dẫn tương đối vào list để lưu DB
                    savedPaths.Add("/images/" + uniqueFileName);
                }
            }

            return savedPaths;
        }

        // GET: /Admin/Sanpham/Edit/5
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

        // POST: /Admin/Sanpham/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Sanpham sanpham, List<IFormFile> Images)
        {
            if (id != sanpham.MaSP) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _sanphamRepository.UpdateAsync(sanpham);

                    // Thêm ảnh mới
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
        public async Task<IActionResult> DeleteImage(int id)  // id = Id của bảng Anh
        {
            var image = _context.Anhs.FirstOrDefault(x => x.Id == id);
            if (image == null)
                return Json(new { success = false, message = "Ảnh không tồn tại" });

            // Xóa file vật lý nếu muốn
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.Anhs.Remove(image);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        // GET: Sanpham/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var sanpham = await _sanphamRepository.GetByIdAsync(id);
            if (sanpham == null) return NotFound();

            return View(sanpham);
        }

        // POST: Sanpham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _sanphamRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Hàm load dropdown
        private void LoadDropdowns()
        {
            ViewBag.MaLoai = new SelectList(_context.Loais.ToList(), "MaLoai", "TenLoai");
            ViewBag.MaNCC = new SelectList(_context.Nhacungcaps.ToList(), "MaNCC", "TenNCC");
            ViewBag.MaKM = new SelectList(_context.Khuyenmais.ToList(), "MaKM", "TenKhuyenMai");
        }
    }
}
