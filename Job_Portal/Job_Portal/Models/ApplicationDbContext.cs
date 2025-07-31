using Job_Portal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Job_Portal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<JobPosting>()
                .HasOne(j => j.Employer)
                .WithMany(u => u.JobPostings)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApplication>()
                .HasOne(a => a.JobSeeker)
                .WithMany(u => u.JobApplications)
                .HasForeignKey(a => a.JobSeekerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApplication>()
                .HasOne(a => a.JobPosting)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobApplication>()
                .Property(a => a.Status)
                .HasDefaultValue(ApplicationStatus.Pending);

            builder.Entity<JobPosting>()
                .HasOne(j => j.Category)
                .WithMany(c => c.JobPostings)
                .HasForeignKey(j => j.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<JobPosting>()
                .HasOne(j => j.Company)
                .WithMany(c => c.JobPostings)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<JobPosting>()
                .Property(j => j.Salary)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "IT" },
                new Category { Id = 2, Name = "Finance" },
                new Category { Id = 3, Name = "Education" }
            );
            builder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Microsoft", Description = "Tech company", Website = "https://microsoft.com" },
                new Company { Id = 2, Name = "Vietcombank", Description = "Banking", Website = "https://vietcombank.com.vn" },
                new Company { Id = 3, Name = "FPT", Description = "IT services", Website = "https://fpt.com.vn" }
            );
        }
    }
}