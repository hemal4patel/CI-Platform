using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using CiPlatformWeb.Repositories.Repository;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace CiPlatformWeb.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly INotification _notification;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly long userId;

        public NotificationController (ApplicationDbContext db, INotification notification, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _notification = notification;

            _httpContextAccessor = httpContextAccessor;
            string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            if (token is not null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadJwtToken(token);
                var claims = decodedToken.Claims;
                var customClaimString = decodedToken.Claims.FirstOrDefault(c => c.Type == "CustomClaimForUser")?.Value;
                var customClaimValue = JsonSerializer.Deserialize<User>(customClaimString);
                userId = customClaimValue.UserId;
            }
        }

        //SHOW ALL NOTIFICATIONS
        public IActionResult GetAllNotifications ()
        {
            List<NotificationParams> list = _notification.GetAllNotifications(userId);
            int unread = _notification.GetUnreadNotificationsCount(userId);

            NotificationModel vm = new NotificationModel
            {
                userNotifications = list,
                UnreadCount = unread
            };
            return PartialView("userNotificationsPartial", vm);
        }

        //MARK NOTIFICATION AS READ
        public IActionResult ChangeNotificationStatus (long id)
        {
            int flag = _notification.ChangeNotificationStatus(id);
            return Json(flag);
        }

        //CLEAR ALL NOTIFICATIONS
        public IActionResult ClearAllNotifications ()
        {
            _notification.ClearAllNotifications(userId);
            return Ok();
        }

        //GET USER SETTINGS FOR NOTIFICATIONS
        public IActionResult GetUserNotificationChanges ()
        {
            long[] settingIds = _notification.GetUserNotificationChanges(userId);
            return Json(settingIds);
        }

        //SAVE USER SETTINGS FOR NOTIFICATIONS
        public IActionResult SaveUserNotificationChanges (long[] settingIds)
        {
            _notification.SaveUserNotificationChanges(userId, settingIds);
            return Ok();
        }
    }
}
