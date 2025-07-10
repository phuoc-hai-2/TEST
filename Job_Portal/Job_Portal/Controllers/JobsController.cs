using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job_Portal.Data;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal.Models; // Đảm bảo bạn có dòng này để truy cập các Models

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
        public async Task<IActionResult> Index(string search, int? categoryId) // Thêm tham số categoryId
        {
            var jobs = _context.JobPostings
                .Include(j => j.Category)
                .Include(j => j.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                jobs = jobs.Where(j => j.Title.Contains(search) || j.Description.Contains(search));
            }

            // Thêm điều kiện lọc theo CategoryId
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                jobs = jobs.Where(j => j.CategoryId == categoryId.Value);
            }

            // Truyền danh sách categories để hiển thị trên View và Layout
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

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