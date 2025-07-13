using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job_Portal.Models
{
    public class JobPosting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [Required]
        [StringLength(50)]
        public string JobType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Workplace { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ApplicationDeadline { get; set; }

        [Required]
        public DateTime PostedDate { get; set; }

        [StringLength(100)]
        public string? DegreeRequirement { get; set; }

        [StringLength(100)]
        public string? ExperienceRequirement { get; set; }

        [StringLength(50)]
        public string? AgeRequirement { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(100)]
        public string? Specialty { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public bool IsClosed { get; set; } = false;

        // Navigation
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public int? CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? Employer { get; set; }
        public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}