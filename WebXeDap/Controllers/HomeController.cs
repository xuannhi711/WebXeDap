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

    public HomeController(ILogger<HomeController> logger, ISanphamRepository sanphamRepository, ILoaiRepository loaiRepository)
    {
        _logger = logger;
        _sanphamRepository = sanphamRepository;
        _loaiRepository = loaiRepository;
    }

    public async Task<IActionResult> Index(string? danhMucId, string? search)
    {
        var sanphams = await _sanphamRepository.GetAllAsync();

        // L?c theo danh m?c n?u có
        if (!string.IsNullOrEmpty(danhMucId))
        {
            sanphams = sanphams.Where(sp => sp.MaLoai == danhMucId).ToList();
        }

        // L?c theo t? khóa tìm ki?m
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
