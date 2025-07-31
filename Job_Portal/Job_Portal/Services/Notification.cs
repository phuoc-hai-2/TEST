using Job_Portal.Data;
using Job_Portal.Models;
using System.Threading.Tasks;

public class NotificationService
{
    private readonly ApplicationDbContext _context;
    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task AddNotification(string userId, string content, string link = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Content = content,
            Link = link
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}