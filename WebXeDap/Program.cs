
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Models;
using WebXeDap.Repositories;
using WebXeDap.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDistributedMemoryCache();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options => 
{ 
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
}); 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options => 
{ 
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
}); 

builder.Services.AddRazorPages();



builder.Services.AddScoped<ILoaiRepository, EFLoaiRepository>();
builder.Services.AddScoped<INhacungcapRepository, EFNhacungcapRepository>();
builder.Services.AddScoped<ISanphamRepository, EFSanphamRepository>();
builder.Services.AddScoped<IKhuyenmaiRepository, EFKhuyenmaiRepository>();
builder.Services.AddScoped<IHoaDonRepository, EFHoaDonRepository>();
builder.Services.AddScoped<IVNPayServices, VNPayService>();



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
// ??t tr??c UseRouting 
app.UseSession();

// Các middleware khác... 
app.UseRouting();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
