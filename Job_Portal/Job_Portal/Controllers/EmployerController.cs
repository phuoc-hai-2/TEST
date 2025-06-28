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
                .Include(j => j.Category)
                .Include(j => j.Company)
                .ToListAsync();

            return View(jobs);
        }

        // GET: /Employer/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Companies = _context.Companies.ToList();
            return View();
        }

        // POST: /Employer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPosting job)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                job.UserId = user.Id;
                job.PostedDate = DateTime.Now;
                _context.JobPostings.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Companies = _context.Companies.ToList();
            return View(job);
        }
    }
}
