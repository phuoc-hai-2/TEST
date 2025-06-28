using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<JobPosting> JobPostings { get; set; }
    }
}
