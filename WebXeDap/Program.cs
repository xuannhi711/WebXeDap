using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;
using WebXeDap.Repositories;
using WebXeDap.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------ db ------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ------------ identity ------------------------------------------
builder
	.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
	{
		options.User.RequireUniqueEmail = true;
	})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders()
	.AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = $"/Identity/Account/Login";
	options.LogoutPath = $"/Identity/Account/Logout";
	options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// ------------ session ------------------------------------------
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// ------------ mvc/razor ------------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ------------ di ------------------------------------------
builder.Services.AddScoped<ILoaiRepository, EFLoaiRepository>();
builder.Services.AddScoped<INhacungcapRepository, EFNhacungcapRepository>();
builder.Services.AddScoped<ISanphamRepository, EFSanphamRepository>();
builder.Services.AddScoped<IKhuyenmaiRepository, EFKhuyenmaiRepository>();
builder.Services.AddScoped<IHoaDonRepository, EFHoaDonRepository>();
builder.Services.AddScoped<IBaohanhRepository, BaohanhRepository>();
builder.Services.AddScoped<IVNPayServices, VNPayService>();
builder.Services.AddScoped<INguoiDungRepository, EFNguoiDungRepository>();
builder.Services.AddScoped<ITintucRepository, TintucRepository>();

// ------------ build app ------------------------------------------
var app = builder.Build();

// ------------ middlewares ------------------------------------------
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // // must be after routing

app.UseAuthentication(); // required for Identity
app.UseAuthorization();

// ------------ endpoints ------------------------------------------
app.MapRazorPages();

app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

// ------------ run ------------------------------------------
app.Run();
