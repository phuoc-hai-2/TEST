using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal.Models
{
    public class JobPosting
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Salary { get; set; }                     // Mức lương
        public string JobType { get; set; }                    // Loại hình (Toàn thời gian,...)
        public string Position { get; set; }                   // Vị trí/chức vụ
        public string DegreeRequirement { get; set; }          // Bằng cấp
        public string ExperienceRequirement { get; set; }      // Yêu cầu kinh nghiệm
        public string AgeRequirement { get; set; }             // Độ tuổi
        public string Industry { get; set; }                   // Ngành nghề
        public string Specialty { get; set; }                  // Chuyên môn
        public string Workplace { get; set; }                  // Địa điểm làm việc
        public DateTime PostedDate { get; set; }               // Ngày đăng tuyển
        public DateTime ApplicationDeadline { get; set; }      // Hạn nộp
        public string Description { get; set; }                // Mô tả công việc (HTML hoặc plaintext)

        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public string UserId { get; set; }
        public ApplicationUser Employer { get; set; }
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}