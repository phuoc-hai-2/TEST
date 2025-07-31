using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Data;
using Job_Portal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Job_Portal.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class JobApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;

        public JobApplicationController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _env = env;
        }

        // 1. Hiển thị danh sách các đơn ứng tuyển của ứng viên hiện tại
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var applications = await _context.JobApplications
                .Where(a => a.JobSeekerId == user.Id)
                .Include(a => a.JobPosting)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();
            return View(applications);
        }

        // 2. Ứng tuyển một công việc (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int jobId)
        {
            // Lấy user hiện tại
            var user = await _userManager.GetUserAsync(User);

            // Kiểm tra công việc có tồn tại không
            var job = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                TempData["ApplyMessage"] = "Công việc không tồn tại.";
                return RedirectToAction("Index", "Jobs");
            }

            // Kiểm tra trạng thái việc làm
            if (job.IsClosed || job.ApplicationDeadline < DateTime.UtcNow)
            {
                TempData["ApplyMessage"] = "Công việc đã đóng hoặc hết hạn ứng tuyển.";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }

            // Kiểm tra đã ứng tuyển chưa
            bool alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobPostingId == jobId && a.JobSeekerId == user.Id);
            if (alreadyApplied)
            {
                TempData["ApplyMessage"] = "Bạn đã ứng tuyển công việc này.";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }

            // Tạo mới application
            var application = new JobApplication
            {
                JobPostingId = jobId,
                JobSeekerId = user.Id,
                AppliedDate = DateTime.UtcNow
            };
            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            // Gửi email xác nhận cho ứng viên
            var templatePath = System.IO.Path.Combine(_env.WebRootPath, "templates", "send2.html");
            if (System.IO.File.Exists(templatePath))
            {
                var html = await System.IO.File.ReadAllTextAsync(templatePath);
                html = html.Replace("{{TenKhachHang}}", user.FullName ?? user.Email)
                           .Replace("{{Email}}", user.Email)
                           .Replace("{{JobTitle}}", job.Title);

                await _emailSender.SendEmailAsync(user.Email, "Xác nhận ứng tuyển", html);
            }

            // Gửi email cho employer (nếu có email)
            if (job.Employer != null && !string.IsNullOrEmpty(job.Employer.Email))
            {
                var templateEmployerPath = System.IO.Path.Combine(_env.WebRootPath, "templates", "send1.html");
                if (System.IO.File.Exists(templateEmployerPath))
                {
                    var htmlEmployer = await System.IO.File.ReadAllTextAsync(templateEmployerPath);
                    htmlEmployer = htmlEmployer.Replace("{{ApplicantName}}", user.FullName ?? user.Email)
                                               .Replace("{{ApplicantEmail}}", user.Email)
                                               .Replace("{{JobTitle}}", job.Title)
                                               .Replace("{{AppliedDate}}", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm"));
                    await _emailSender.SendEmailAsync(job.Employer.Email, "Có ứng viên mới ứng tuyển", htmlEmployer);
                }
            }

            TempData["ApplyMessage"] = "Ứng tuyển thành công! Đã gửi email xác nhận cho bạn.";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }
    }
}