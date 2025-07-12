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
        public async Task<IActionResult> Index()
        {
            var jobs = await _context.JobPostings
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
            return View(jobs);
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