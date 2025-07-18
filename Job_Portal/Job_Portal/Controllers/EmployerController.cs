using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Data;
using Job_Portal.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Job_Portal.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Employer/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var jobs = await _context.JobPostings
                .Where(j => j.UserId == user.Id)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            return View(jobs);
        }

        // GET: /Employer/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Employer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPosting model)
        {
            // Xử lý Category
            var categoryName = Request.Form["Category.Name"].ToString();
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
                if (category == null)
                {
                    category = new Category { Name = categoryName };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                }
                model.CategoryId = category.Id;
            }

            // Xử lý Company (cho phép nhập tên mới)
            var companyName = Request.Form["CompanyName"].ToString();
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == companyName);
                if (company == null)
                {
                    company = new Company { Name = companyName };
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }
                model.CompanyId = company.Id;
            }

            model.PostedDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                model.UserId = user.Id;
                _context.JobPostings.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == user.Id);
            if (job == null) return NotFound();

            return View(job);
        }

        // POST: /Employer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobPosting model)
        {
            var user = await _userManager.GetUserAsync(User);
            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == user.Id);
            if (job == null) return NotFound();

            var categoryName = Request.Form["Category.Name"].ToString();
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
                if (category == null)
                {
                    category = new Category { Name = categoryName };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                }
                job.CategoryId = category.Id;
            }

            var companyName = Request.Form["CompanyName"].ToString();
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == companyName);
                if (company == null)
                {
                    company = new Company { Name = companyName };
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }
                job.CompanyId = company.Id;
            }
            else
            {
                job.CompanyId = model.CompanyId;
            }

            if (ModelState.IsValid)
            {
                job.Title = model.Title;
                job.Salary = model.Salary;
                job.JobType = model.JobType;
                job.Position = model.Position;
                job.DegreeRequirement = model.DegreeRequirement;
                job.ExperienceRequirement = model.ExperienceRequirement;
                job.AgeRequirement = model.AgeRequirement;
                job.Industry = model.Industry;
                job.Specialty = model.Specialty;
                job.Workplace = model.Workplace;
                job.ApplicationDeadline = model.ApplicationDeadline;
                job.Description = model.Description;

                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }
            return View(model);
        }

        public async Task<IActionResult> Applications(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var job = await _context.JobPostings
                .Include(j => j.Applications)
                .ThenInclude(a => a.JobSeeker)
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == user.Id);

            if (job == null) return NotFound();

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmInterview(int id, int applicationId)
        {
            var application = await _context.JobApplications
                .Include(a => a.JobSeeker)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostingId == id);

            if (application != null && application.Status == ApplicationStatus.Pending)
            {
                application.Status = ApplicationStatus.Invited;
                application.FeedbackMessage = "Bạn đã được mời tham gia phỏng vấn. Vui lòng kiểm tra email hoặc liên hệ nhà tuyển dụng để biết thêm chi tiết!";
                await _context.SaveChangesAsync();
                // TODO: Gửi email nếu muốn
            }
            return RedirectToAction("Applications", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCandidate(int id, int applicationId)
        {
            var application = await _context.JobApplications
                .Include(a => a.JobSeeker)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobPostingId == id);

            if (application != null && application.Status == ApplicationStatus.Pending)
            {
                application.Status = ApplicationStatus.Rejected;
                application.FeedbackMessage = "Rất tiếc, bạn chưa được chọn cho vòng phỏng vấn. Chúc bạn may mắn lần sau!";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Applications", new { id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == user.Id);

            if (job == null) return NotFound();

            _context.JobPostings.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> CandidateDetail(int id, int applicationId)
        {
            var job = await _context.JobPostings
                .Include(j => j.Applications)
                .ThenInclude(a => a.JobSeeker)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            var application = job.Applications.FirstOrDefault(a => a.Id == applicationId);
            if (application == null) return NotFound();

            return View("CandidateDetail", application);
        }
    }
}