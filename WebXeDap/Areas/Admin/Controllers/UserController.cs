using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models;
using WebXeDap.Repositories;

namespace WebXeDap.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = USER_ROLES.ADMIN)]
    public class UserController : Controller
    {
        private readonly INguoiDungRepository _nguoiDungRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(INguoiDungRepository nguoiDungRepo, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _nguoiDungRepo.GetAllAsync();
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> QuanLy()
        {
            var users = await _nguoiDungRepo.GetAllAsync();
            var QuanLy = new List<ApplicationUser>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin") || roles.Contains("Employee"))
                {
                    QuanLy.Add(user);
                }
            }

            return View(QuanLy);
        }
        public async Task<IActionResult> Khachhang()
        {
            var users = await _nguoiDungRepo.GetAllAsync();
            var Khachhang = new List<ApplicationUser>();
            var newCustomersCount = 0;
            var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Customer"))
                {
                    Khachhang.Add(user);
                    if (user.CreatedDate >= threeDaysAgo)
                    {
                        newCustomersCount++;
                    }
                }
            }

            ViewBag.NewCustomersCount = newCustomersCount;
            return View(Khachhang);
        }
        public async Task<IActionResult> Details(string id)
        {
            var user = await _nguoiDungRepo.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _nguoiDungRepo.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                await _nguoiDungRepo.UpdateAsync(user);
                return RedirectToAction("CustomerList");
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _nguoiDungRepo.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _nguoiDungRepo.DeleteAsync(id);
            return RedirectToAction("CustomerList");
        }
    }
}
