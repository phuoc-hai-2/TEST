using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public class JobPosting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Required]
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public string? Description { get; set; } // <-- BỔ SUNG DÒNG NÀY

        // Foreign Keys
        public int? CategoryId { get; set; }
        public int? CompanyId { get; set; }
        [Required]
        public string UserId { get; set; } // Employer Id

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual Company Company { get; set; }
        public virtual ApplicationUser Employer { get; set; }

        public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}