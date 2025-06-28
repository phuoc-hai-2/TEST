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

        public string Description { get; set; }
        public string Website { get; set; }

        public ICollection<JobPosting> JobPostings { get; set; }
    }
}