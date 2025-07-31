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

        // GET: /JobApplication/Index
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

        // POST: /JobApplication/Create/{jobId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int jobId)
        {
            var user = await _userManager.GetUserAsync(User);
            var job = await _context.JobPostings.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == jobId);
            if (job == null)
            {
                TempData["ApplyMessage"] = "Công việc không tồn tại.";
                return RedirectToAction("Index", "Jobs");
            }

            if (job.IsClosed || job.ApplicationDeadline < DateTime.UtcNow)
            {
                TempData["ApplyMessage"] = "Công việc đã đóng hoặc hết hạn ứng tuyển.";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }

            bool alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobPostingId == jobId && a.JobSeekerId == user.Id);
            if (alreadyApplied)
            {
                TempData["ApplyMessage"] = "Bạn đã ứng tuyển công việc này.";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }

            var application = new JobApplication
            {
                JobPostingId = jobId,
                JobSeekerId = user.Id,
                AppliedDate = DateTime.UtcNow
            };
            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            // --- GỬI EMAIL XÁC NHẬN ỨNG TUYỂN ---
            var templatePath = System.IO.Path.Combine(_env.WebRootPath, "templates", "send2.html");
            var html = await System.IO.File.ReadAllTextAsync(templatePath);
            html = html.Replace("{{TenKhachHang}}", user.FullName ?? user.Email)
                       .Replace("{{Email}}", user.Email)
                       .Replace("{{JobTitle}}", job.Title);
            await _emailSender.SendEmailAsync(user.Email, "Xác nhận ứng tuyển", html);

            TempData["ApplyMessage"] = "Ứng tuyển thành công! Đã gửi email xác nhận cho bạn.";
            return RedirectToAction("Details", "Jobs", new { id = jobId });

            if (job.Employer != null && !string.IsNullOrEmpty(job.Employer.Email))
            {
                var templateEmployerPath = System.IO.Path.Combine(_env.WebRootPath, "templates", "send1.html");
                var htmlEmployer = await System.IO.File.ReadAllTextAsync(templateEmployerPath);
                htmlEmployer = htmlEmployer.Replace("{{ApplicantName}}", user.FullName ?? user.Email)
                                           .Replace("{{ApplicantEmail}}", user.Email)
                                           .Replace("{{JobTitle}}", job.Title)
                                           .Replace("{{AppliedDate}}", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm"));
                await _emailSender.SendEmailAsync(job.Employer.Email, "Có ứng viên mới ứng tuyển", htmlEmployer);
            }

            TempData["ApplyMessage"] = "Ứng tuyển thành công! Đã gửi email xác nhận cho bạn.";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }
    }
}