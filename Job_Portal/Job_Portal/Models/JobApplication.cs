using System;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public enum ApplicationStatus
    {
        Pending = 0,
        Invited = 1, // Được mời phỏng vấn
        Rejected = 2 // Đã loại
    }

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

        // Thêm trạng thái và thông báo
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        [StringLength(500)]
        public string? FeedbackMessage { get; set; }
    }
}