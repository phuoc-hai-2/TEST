using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Job_Portal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Job_Portal.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Họ và tên")]
            [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
            [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
            public string FullName { get; set; }

            [Display(Name = "Kỹ năng")]
            public string Skills { get; set; }

            [Display(Name = "Trình độ học vấn")]
            public string Qualifications { get; set; }

            [Display(Name = "Tên công ty")]
            public string CompanyName { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FullName = user.FullName,
                Skills = user.Skills,
                Qualifications = user.Qualifications,
                CompanyName = user.CompanyName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không tìm thấy user với ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không tìm thấy user với ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Có lỗi khi cập nhật số điện thoại.";
                    return RedirectToPage();
                }
            }

            user.FullName = Input.FullName;

            if (User.IsInRole("JobSeeker"))
            {
                user.Skills = Input.Skills;
                user.Qualifications = Input.Qualifications;
            }
            if (User.IsInRole("Employer"))
            {
                user.CompanyName = Input.CompanyName;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                StatusMessage = "Có lỗi khi cập nhật hồ sơ.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Cập nhật tài khoản thành công";
            return RedirectToPage();
        }
    }
}