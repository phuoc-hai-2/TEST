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

// 2. Cấu hình Identity một cách đầy đủ và mạnh mẽ
// Sử dụng AddDefaultIdentity để nạp tất cả các dịch vụ cần thiết cho UI
// Sau đó, thêm Roles để quản lý vai trò.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // <-- Thêm quản lý vai trò vào cấu hình mặc định
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Đăng ký dịch vụ EmailSender (quan trọng)
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 3. Thêm Razor Pages và MVC
builder.Services.AddControllersWithViews();
// Thêm AddRazorPagesOptions để đảm bảo các trang trong Area được tìm thấy
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
});


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

// Sử dụng UseAuthentication() và UseAuthorization() là đủ
app.UseAuthentication();
app.UseAuthorization();

// 5. Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Jobs}/{action=Index}/{id?}"); // Thay Home bằng Jobs làm mặc định

app.MapRazorPages(); // Quan trọng: Phải có để Identity UI hoạt động

app.Run();