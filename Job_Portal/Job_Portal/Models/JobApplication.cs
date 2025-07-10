using System;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

        [Required]
        public int JobPostingId { get; set; }

        [Required]
        public string UserId { get; set; } // JobSeeker Id

        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual JobPosting JobPosting { get; set; }
        public virtual ApplicationUser JobSeeker { get; set; }
    }
}