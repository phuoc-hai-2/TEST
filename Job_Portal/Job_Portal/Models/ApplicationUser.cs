using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Job_Portal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string? Skills { get; set; }
        public string? Qualifications { get; set; }

        // *** THÊM THUỘC TÍNH MỚI CHO EMPLOYER ***
        public string? CompanyName { get; set; }

        public ICollection<JobApplication> JobApplications { get; set; }

        public ICollection<JobPosting> JobPostings { get; set; }
    }
}