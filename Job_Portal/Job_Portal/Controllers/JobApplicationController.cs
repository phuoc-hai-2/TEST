using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Job_Portal.Data;
using Job_Portal.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int jobId)
        {
            var user = await _userManager.GetUserAsync(User);

            var alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobPostingId == jobId && a.UserId == user.Id);
            if (alreadyApplied)
            {
                TempData["Message"] = "Bạn đã ứng tuyển công việc này.";
                return RedirectToAction("Index", "Jobs");
            }

            var application = new JobApplication
            {
                JobPostingId = jobId,
                UserId = user.Id,
                AppliedDate = DateTime.UtcNow
                // ResumeFilePath = "", // Thêm nếu bạn có property này
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Ứng tuyển thành công!";
            return RedirectToAction("Index", "Jobs");
        }
    }
}