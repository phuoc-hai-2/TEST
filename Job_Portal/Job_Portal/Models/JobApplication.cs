using System;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

        [Required]
        public int JobPostingId { get; set; }
        public virtual JobPosting JobPosting { get; set; }

        [Required]
        public string JobSeekerId { get; set; }
        public virtual ApplicationUser JobSeeker { get; set; }

        [Required]
        public DateTime AppliedDate { get; set; }
    }
}