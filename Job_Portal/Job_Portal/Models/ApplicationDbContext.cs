using Job_Portal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet đại diện cho các bảng trong CSDL
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Company> Companies { get; set; }

        // Không override OnModelCreating hoặc giữ nguyên base
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // JobPosting → ApplicationUser
            builder.Entity<JobPosting>()
                .HasOne(j => j.Employer)
                .WithMany(u => u.JobPostings)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc .NoAction()

            // JobApplication → ApplicationUser
            builder.Entity<JobApplication>()
                .HasOne(a => a.JobSeeker)
                .WithMany(u => u.JobApplications)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // JobApplication → JobPosting
            builder.Entity<JobApplication>()
                .HasOne(a => a.JobPosting)
                .WithMany()
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade); // giữ lại Cascade ở đây
        }

    }
}
