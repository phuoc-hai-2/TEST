using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Data;
using Job_Portal.Models;
using System.Threading.Tasks;
using System.Linq;

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
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                // Nếu cần xử lý Category/Company động (nếu có)
                // Nếu không dùng, có thể bỏ các đoạn này
                Category category = null;
                if (!string.IsNullOrWhiteSpace(model.Category?.Name))
                {
                    category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == model.Category.Name);
                    if (category == null)
                    {
                        category = new Category { Name = model.Category.Name };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }
                    model.CategoryId = category.Id;
                }

                Company company = null;
                if (!string.IsNullOrWhiteSpace(model.Company?.Name))
                {
                    company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == model.Company.Name);
                    if (company == null)
                    {
                        company = new Company { Name = model.Company.Name };
                        _context.Companies.Add(company);
                        await _context.SaveChangesAsync();
                    }
                    model.CompanyId = company.Id;
                }

                model.UserId = user.Id;
                model.PostedDate = DateTime.UtcNow;

                _context.JobPostings.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Dashboard");
            }
            return View(model);
        }

        // GET: /Employer/Edit/5
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

            if (ModelState.IsValid)
            {
                // Nếu cần xử lý category/company động thì thêm code như ở Create

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
                // Không đổi PostedDate/UserId

                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }
            return View(model);
        }

        // Xem ứng viên của một tin tuyển dụng
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
    }
}