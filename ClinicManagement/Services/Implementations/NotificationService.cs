using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        public NotificationService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
            => await _context.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToListAsync();

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
            => await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).OrderByDescending(n => n.CreatedAt).Take(5).ToListAsync();

        public async Task<int> GetUnreadCountAsync(string userId)
            => await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task CreateNotificationAsync(string userId, string title, string message, NotificationType type)
        {
            _context.Notifications.Add(new Notification { UserId = userId, Title = title, Message = message, Type = type, CreatedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var n = await _context.Notifications.FindAsync(notificationId);
            if (n != null) { n.IsRead = true; await _context.SaveChangesAsync(); }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unread = await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
            foreach (var n in unread) n.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}
