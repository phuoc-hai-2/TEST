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
        }
    }
}