using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models;
using System.Linq;

namespace WebXeDap.Controllers
{
    public class TintucController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TintucController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tinTucs = _context.tintuc
                .Where(t => !t.tieude.ToLower().Contains("poster"))
                .OrderByDescending(t => t.ngaytao)
                .ToList();

            return View(tinTucs);
        }

        public IActionResult Details(int id)
        {
            var tin = _context.tintuc.FirstOrDefault(t => t.id == id && !t.tieude.ToLower().Contains("poster"));
            if (tin == null)
                return NotFound();

            return View(tin);
        }
    }
}
