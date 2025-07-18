using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Data;
using System.Threading.Tasks;
using System.Linq;

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
        public async Task<IActionResult> Index(string keyword, string location, string jobType, decimal? minSalary, decimal? maxSalary)
        {
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

            var jobList = await jobs.OrderByDescending(j => j.PostedDate).ToListAsync();
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