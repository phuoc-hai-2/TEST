using Job_Portal.Data;
using Job_Portal.Models;
using Job_Portal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DbContext dùng SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dòng này gây trùng lặp và cần được xóa bỏ:
// builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// 2. Cấu hình Identity đầy đủ và đúng cách (đã bao gồm hỗ trợ vai trò)
// *** ĐÂY LÀ DÒNG CHÍNH XÁC ***
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false) // Chuyển từ true sang false
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>(); // Đảm bảo hỗ trợ vai trò

// Đăng ký dịch vụ EmailSender
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 3. Thêm Razor Pages và MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 4. Cấu hình Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 5. Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Jobs}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();