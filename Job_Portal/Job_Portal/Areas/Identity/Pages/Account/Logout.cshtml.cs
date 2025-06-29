// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Job_Portal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Job_Portal.Areas.Identity.Pages.Account
{
    [AllowAnonymous] // Đảm bảo trang này có thể truy cập được
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // Phương thức xử lý khi người dùng bấm nút Logout (POST)
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                // Nếu có đường dẫn trả về, chuyển hướng đến đó
                return LocalRedirect(returnUrl);
            }
            else
            {
                // *** SỬA LỖI Ở ĐÂY ***
                // Nếu không, luôn chuyển hướng về trang chủ một cách an toàn
                return RedirectToAction("Index", "Jobs", new { area = "" });
            }
        }

        // Thêm phương thức để xử lý khi người dùng truy cập trực tiếp (GET)
        public async Task<IActionResult> OnGet()
        {
            // Thực hiện đăng xuất và chuyển hướng về trang chủ
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out via GET request.");
            return RedirectToAction("Index", "Jobs", new { area = "" });
        }
    }
}