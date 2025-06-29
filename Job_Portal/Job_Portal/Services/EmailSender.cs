using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Job_Portal.Services
{
    // Lớp giả này chỉ để ứng dụng có thể chạy.
    // Trong thực tế, bạn sẽ cần tích hợp một dịch vụ email thật như SendGrid, Mailgun,...
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Hiện tại chúng ta không làm gì cả. 
            // Email sẽ không được gửi đi, nhưng lỗi sẽ biến mất.
            // Trong tương lai, bạn sẽ viết logic gửi email thật ở đây.
            return Task.CompletedTask;
        }
    }
}