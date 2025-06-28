using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Job_Portal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Skills { get; set; }         // Dành cho Job Seeker
        public string Qualifications { get; set; } // Dành cho Job Seeker

        // Liên kết với JobApplication nếu là Job Seeker
        public ICollection<JobApplication> JobApplications { get; set; }

        // Liên kết với JobPosting nếu là Employer
        public ICollection<JobPosting> JobPostings { get; set; }
    }
}
