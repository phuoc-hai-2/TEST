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

            model.PostedDate = DateTime.UtcNow;

            // In ModelState lỗi ra Output
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"{key}: {error.ErrorMessage}");
                }
            }

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

            // Lấy tên category và company nhập từ form (nếu có)
            var categoryName = Request.Form["Category.Name"].ToString();
            var companyName = Request.Form["Company.Name"].ToString();

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