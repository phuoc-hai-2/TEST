using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Job_Portal.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}