using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Job_Portal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string? Skills { get; set; }
        public string? Qualifications { get; set; }
        public string? CompanyName { get; set; } // Chỉ cho Employer

        // Navigation properties
        public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
        public virtual ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    }
}