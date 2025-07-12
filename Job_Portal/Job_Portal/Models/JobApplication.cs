using System;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public int JobPostingId { get; set; }
        public JobPosting JobPosting { get; set; }
        public string JobSeekerId { get; set; }
        public ApplicationUser JobSeeker { get; set; }
        public DateTime AppliedDate { get; set; }
    }
}