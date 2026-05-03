using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClinicManagement.Models.Entities;
using ClinicManagement.Services.Interfaces;

namespace ClinicManagement.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        { _notificationService = notificationService; _userManager = userManager; }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications = await _notificationService.GetUserNotificationsAsync(user!.Id);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            var user = await _userManager.GetUserAsync(User);
            await _notificationService.MarkAllAsReadAsync(user!.Id);
            TempData["Success"] = "All notifications marked as read.";
            return RedirectToAction("Index");
        }

        // API for navbar bell
        [HttpGet]
        public async Task<IActionResult> GetUnread()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { count = 0, items = Array.Empty<object>() });
            var count = await _notificationService.GetUnreadCountAsync(user.Id);
            var items = await _notificationService.GetUnreadNotificationsAsync(user.Id);
            return Json(new
            {
                count,
                items = items.Select(n => new { n.NotificationId, n.Title, n.Message, Time = n.CreatedAt.ToString("MMM dd, HH:mm"), n.Type })
            });
        }
    }
}
