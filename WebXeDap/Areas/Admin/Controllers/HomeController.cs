using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INguoiDungRepository _nguoiDungRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ApplicationDbContext context, INguoiDungRepository nguoiDungRepo, UserManager<ApplicationUser> userManager)
        {
            _context = context; _nguoiDungRepo = nguoiDungRepo;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

       
    }
}
