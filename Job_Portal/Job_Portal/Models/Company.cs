using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Website { get; set; }

        public virtual ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    }
}