using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Data;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Job_Portal.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Jobs
        public async Task<IActionResult> Index(
            string keyword, string location, string jobType, decimal? minSalary, decimal? maxSalary, int page = 1)
        {
            int pageSize = 6;

            var jobs = _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                jobs = jobs.Where(j =>
                    j.Title.Contains(keyword) ||
                    j.Position.Contains(keyword) ||
                    (j.Company != null && j.Company.Name.Contains(keyword))
                );
            if (!string.IsNullOrWhiteSpace(location))
                jobs = jobs.Where(j => j.Workplace.Contains(location));
            if (!string.IsNullOrWhiteSpace(jobType))
                jobs = jobs.Where(j => j.JobType == jobType);
            if (minSalary.HasValue)
                jobs = jobs.Where(j => j.Salary >= minSalary);
            if (maxSalary.HasValue)
                jobs = jobs.Where(j => j.Salary <= maxSalary);

            int totalJobs = await jobs.CountAsync();
            var jobList = await jobs.OrderByDescending(j => j.PostedDate)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalJobs / pageSize);

            ViewBag.Keyword = keyword;
            ViewBag.Location = location;
            ViewBag.JobType = jobType;
            ViewBag.MinSalary = minSalary;
            ViewBag.MaxSalary = maxSalary;

            return View(jobList);
        }

        // GET: /Jobs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();
            return View(job);
        }
    }
}