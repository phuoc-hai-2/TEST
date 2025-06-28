using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Data;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(string search)
        {
            var jobs = _context.JobPostings
                .Include(j => j.Category)
                .Include(j => j.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                jobs = jobs.Where(j => j.Title.Contains(search) || j.Description.Contains(search));
            }

            return View(await jobs.ToListAsync());
        }

        // GET: /Jobs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.Category)
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            return View(job);
        }
    }
}
