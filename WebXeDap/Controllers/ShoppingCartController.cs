using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebXeDap.Extensions;
using WebXeDap.Models;
using WebXeDap.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Services;
using WebXeDap.Models.VNPay;

namespace WebXeDap.Controllers
{
    //[Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ISanphamRepository _sanphamRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVNPayServices _vnpayService;

        public ShoppingCartController(ApplicationDbContext context,
                                      UserManager<ApplicationUser> userManager,
                                      ISanphamRepository sanphamRepository,
                                      IVNPayServices vnpayService)
        {
            _sanphamRepository = sanphamRepository;
            _context = context;
            _userManager = userManager;
            _vnpayService = vnpayService;
        }

        public async Task<IActionResult> AddToCart(string masp, int sl, string mau)
        {
            if (sl <= 0) sl = 1;
            var product = await _sanphamRepository.GetByIdAsync(masp);
            if (product == null) return NotFound();

            var firstImage = product.Anhs?.FirstOrDefault()?.Url ?? "/images/default.png";
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId)) // Đã đăng nhập → Lưu vào database
            {
                var cartItem = new Giohang
                {
                    UserId = userId,
                    MaSP = masp,
                    SoLuong = sl,
                    Mau = mau,
                    HinhAnh = firstImage,
                    DonGia = product.GiaBan
                };

                // Check if the item already exists in the cart
                var existingCartItem = await _context.Giohangs
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.MaSP == masp && c.Mau == mau);

                if (existingCartItem != null)
                {
                    existingCartItem.SoLuong += sl;
                    _context.Giohangs.Update(existingCartItem);
                }
                else
                {
                    _context.Giohangs.Add(cartItem);
                }
                await _context.SaveChangesAsync();
            }
            else // Chưa đăng nhập → Lưu vào Session
            {
                var cartItem = new Giohang
                {
                    MaSP = masp,
                    SoLuong = sl,
                    Mau = mau,
                    HinhAnh = firstImage,
                    DonGia = product.GiaBan
                };

                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
                cart.AddItem(cartItem);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }





        //public IActionResult TangSL(string productId)
        //{
        //    var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        //    if (cart != null)
        //    {
        //        var item = cart.Items.FirstOrDefault(i => i.MaSP == productId);
        //        if (item != null)
        //        {
        //            item.SoLuong++;
        //            HttpContext.Session.SetObjectAsJson("Cart", cart);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        //public IActionResult GiamSL(string productId)
        //{
        //    var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        //    if (cart != null)
        //    {
        //        var item = cart.Items.FirstOrDefault(i => i.MaSP == productId);
        //        if (item != null && item.SoLuong > 1)
        //        {
        //            item.SoLuong--;
        //            HttpContext.Session.SetObjectAsJson("Cart", cart);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> TangSL(string productId)
        {
            string userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            if (!string.IsNullOrEmpty(userId)) // Nếu đã đăng nhập, cập nhật database
            {
                var cartItem = await _context.Giohangs
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.MaSP == productId);
                if (cartItem != null)
                {
                    cartItem.SoLuong++;
                    _context.Giohangs.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
            }
            else // Nếu chưa đăng nhập, cập nhật session
            {
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
                if (cart != null)
                {
                    var item = cart.Items.FirstOrDefault(i => i.MaSP == productId);
                    if (item != null)
                    {
                        item.SoLuong++;
                        HttpContext.Session.SetObjectAsJson("Cart", cart);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GiamSL(string productId)
        {
            string userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            if (!string.IsNullOrEmpty(userId)) // Nếu đã đăng nhập, cập nhật database
            {
                var cartItem = await _context.Giohangs
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.MaSP == productId);
                if (cartItem != null && cartItem.SoLuong > 1)
                {
                    cartItem.SoLuong--;
                    _context.Giohangs.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
            }
            else // Nếu chưa đăng nhập, cập nhật session
            {
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
                if (cart != null)
                {
                    var item = cart.Items.FirstOrDefault(i => i.MaSP == productId);
                    if (item != null && item.SoLuong > 1)
                    {
                        item.SoLuong--;
                        HttpContext.Session.SetObjectAsJson("Cart", cart);
                    }
                }
            }
            return RedirectToAction("Index");
        }











        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShoppingCart cart = new ShoppingCart();

            if (!string.IsNullOrEmpty(userId)) // Nếu đã đăng nhập, lấy từ database
            {
                var cartItems = await _context.Giohangs
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                foreach (var item in cartItems)
                {
                    cart.AddItem(new Giohang
                    {
                        MaSP = item.MaSP,
                        SoLuong = item.SoLuong,
                        Mau = item.Mau,
                        HinhAnh = item.HinhAnh,
                        DonGia = item.DonGia
                    });
                }
            }
            else // Nếu chưa đăng nhập, lấy từ Session
            {
                cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            }

            return View(cart);
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(string masp)
        {
            if (string.IsNullOrEmpty(masp))
            {
                return BadRequest("Mã sản phẩm không hợp lệ.");
            }

            string userId = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;

            if (!string.IsNullOrEmpty(userId))
            {
                // Nếu đã đăng nhập, xóa trong database
                var cartItem = await _context.Giohangs.FirstOrDefaultAsync(c => c.MaSP == masp);
                if (cartItem == null)
                {
                    return NotFound("Sản phẩm không tồn tại trong giỏ hàng.");
                }

                _context.Giohangs.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
                cart.RemoveItem(masp);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index", "ShoppingCart");
        }





        //public IActionResult Checkout()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    ShoppingCart cart = new ShoppingCart();

        //    if (!string.IsNullOrEmpty(userId)) // Nếu đã đăng nhập, lấy từ database
        //    {
        //        var cartItems = _context.Giohangs
        //            .Where(c => c.UserId == userId)
        //            .Include(c => c.Sanpham)
        //            .ToList();

        //        foreach (var item in cartItems)
        //        {
        //            cart.AddItem(new Giohang
        //            {
        //                MaSP = item.MaSP,
        //                DonGia = item.DonGia,
        //                SoLuong = item.SoLuong,
        //                HinhAnh = item.HinhAnh,
        //                Mau = item.Mau
        //            });
        //        }
        //    }
        //    else // Nếu chưa đăng nhập, lấy từ Session
        //    {
        //        cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
        //    }

        //    if (!cart.Items.Any())
        //    {
        //        // Giỏ hàng trống, chuyển hướng về trang giỏ hàng
        //        return RedirectToAction("Index");
        //    }

        //    return View(cart);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Checkout(Hoadon order, string payment)
        //{ 
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    ShoppingCart cart = new ShoppingCart();

        //    if (!string.IsNullOrEmpty(userId)) 
        //    {
        //        var cartItems = _context.Giohangs
        //            .Where(c => c.UserId == userId)
        //            .Include(c => c.Sanpham)
        //            .ToList();

        //        foreach (var item in cartItems)
        //        {
        //            cart.AddItem(new Giohang
        //            {
        //                MaSP = item.MaSP,
        //                DonGia = item.DonGia,
        //                SoLuong = item.SoLuong,
        //                HinhAnh = item.HinhAnh,
        //                Mau = item.Mau
        //            });
        //        }
        //    }
        //    else // Nếu chưa đăng nhập, lấy từ Session
        //    {
        //        cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
        //    }

        //    if (!cart.Items.Any())
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Index");
        //    }


        //    order.UserId = user.Id;
        //    order.NgayLapHD = DateTime.UtcNow;
        //    order.TongTien = cart.Items.Sum(i => i.DonGia * i.SoLuong);
        //    order.Chitiethoadons = cart.Items.Select(i => new Chitiethoadon
        //    {
        //        MaSP = i.MaSP,
        //        SoLuong = i.SoLuong,
        //        Mausac = i.Mau,
        //        DonGia = i.DonGia
        //    }).ToList();

        //    _context.Hoadon.Add(order);
        //    await _context.SaveChangesAsync(); 


        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        var cartItems = _context.Giohangs.Where(c => c.UserId == userId).ToList();
        //        _context.Giohangs.RemoveRange(cartItems);
        //        await _context.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        HttpContext.Session.Remove("Cart");
        //    }


        //    return View("OrderCompleted", order);
        //}

    }
}
