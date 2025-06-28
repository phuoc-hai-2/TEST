using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job_Portal.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

        [Required]
        public int JobPostingId { get; set; }
        [ForeignKey("JobPostingId")]
        public JobPosting JobPosting { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser JobSeeker { get; set; }

        public DateTime AppliedDate { get; set; } = DateTime.Now;
        public string ResumeFilePath { get; set; }
    }
}
