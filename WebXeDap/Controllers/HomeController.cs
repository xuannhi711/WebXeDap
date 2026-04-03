using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Controllers;

public class HomeController : Controller
{ private readonly ISanphamRepository _sanphamRepository;
    private readonly ILogger<HomeController> _logger;
    private readonly ILoaiRepository _loaiRepository;
    private readonly ApplicationDbContext _context;
    public HomeController(ILogger<HomeController> logger, ISanphamRepository sanphamRepository, ILoaiRepository loaiRepository, ApplicationDbContext context)
    {
        _logger = logger;
        _sanphamRepository = sanphamRepository;
        _loaiRepository = loaiRepository;
        _context = context;
    }

    public async Task<IActionResult> Index(string? danhMucId, string? search)
    {
        var sanphams = await _sanphamRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(danhMucId))
        {
            sanphams = sanphams.Where(sp => sp.MaLoai == danhMucId).ToList();
        }

        if (!string.IsNullOrEmpty(search))
        {
            sanphams = sanphams.Where(sp =>
                sp.TenSP.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                sp.MaSP.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (sp.Loai != null && sp.Loai.TenLoai.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (sp.Nhacungcap != null && sp.Nhacungcap.TenNCC.Contains(search, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        ViewBag.DanhMuc = await _loaiRepository.GetAllAsync();

        // Thêm: lấy danh sách poster từ tintuc
        var posters = await _context.tintuc
            .Where(t => t.tieude.ToLower() == "poster" && t.hinhanh != null)
            .ToListAsync();
        ViewBag.Posters = posters;

        return View(sanphams);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
