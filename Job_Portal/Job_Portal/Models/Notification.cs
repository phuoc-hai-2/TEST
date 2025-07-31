using System;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } // Ai nhận thông báo
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string Content { get; set; }

        public string Link { get; set; } // Link đi đến chi tiết (ví dụ: chi tiết job, ứng tuyển...)

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}