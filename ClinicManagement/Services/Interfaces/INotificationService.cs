using ClinicManagement.Models.Entities;
using ClinicManagement.Models.Enums;

namespace ClinicManagement.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task CreateNotificationAsync(string userId, string title, string message, NotificationType type);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
