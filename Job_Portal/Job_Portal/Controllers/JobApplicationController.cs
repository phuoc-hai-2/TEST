using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Data;
using Job_Portal.Models;
using Microsoft.AspNetCore.Identity;
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

        public JobApplicationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var job = await _context.JobPostings.FindAsync(jobId);
            if (job == null)
            {
                TempData["ApplyMessage"] = "Công việc không tồn tại.";
                return RedirectToAction("Index", "Jobs");
            }

            // Kiểm tra trạng thái job posting
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

            var application = new JobApplication
            {
                JobPostingId = jobId,
                JobSeekerId = user.Id,
                AppliedDate = DateTime.UtcNow
            };
            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["ApplyMessage"] = "Ứng tuyển thành công!";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }
    }
}