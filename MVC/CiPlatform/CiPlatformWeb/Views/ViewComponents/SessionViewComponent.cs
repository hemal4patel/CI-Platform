using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace CiPlatformWeb.Views.ViewComponents
{
    public class SessionViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public SessionViewComponent (ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync (string view)
        {
            long userId = 0;
            string userName = "";
            string emailId = "";
            string avatarName = null;
            string role = "";

            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
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

            if (userId != 0)
            {
                User sessionUser = _db.Users.Where(user => user.UserId == userId).FirstOrDefault();
                userId = userId;
                userName = sessionUser.FirstName + " " + sessionUser.LastName;
                emailId = sessionUser.Email;
                role = sessionUser.Role;
                if (sessionUser.Avatar is not null)
                {
                    avatarName = sessionUser.Avatar;
                }
                else
                {
                    avatarName = null;
                }
            }

            SessionUserViewModel vm = new();
            vm.userId = userId;
            vm.userName = userName;
            vm.emailId = emailId;
            vm.role = role;
            vm.avatarName = avatarName;
            vm.cmsPages = _db.CmsPages.Where(cms => cms.DeletedAt == null).ToList();
            vm.userNotifications = getUserNotification(userId);

            return View(view, vm);
        }

        public List<NotificationParams> getUserNotification (long userId)
        {
            IQueryable<UserNotification> notifications = _db.UserNotifications.Where(u => u.ToUserId == userId && u.DeletedAt == null).OrderByDescending(u => u.CreatedAt).AsQueryable();

            var list = notifications.Select(n => new NotificationParams()
            {
                notificationId = n.NotificationId,
                toUserId = userId,
                fromUserId = n.FromUserId.HasValue ? n.FromUserId : 0,
                recommendedMissionId = n.RecommendedMissionId.HasValue ? n.RecommendedMissionId : 0,
                recommendedStoryId = n.RecommendedStooyId.HasValue ? n.RecommendedStooyId : 0,
                newMissionId = n.NewMissionId.HasValue ? n.NewMissionId : 0,
                storyId = n.StoryId.HasValue ? n.StoryId : 0,
                timesheetId = n.TimesheetId.HasValue ? n.TimesheetId : 0,
                commentId = n.CommentId.HasValue ? n.CommentId : 0,
                MissionApplicationId = n.MissionApplicationId.HasValue ? n.MissionApplicationId : 0,
                status = n.Status,
                createdAt = n.CreatedAt,
                fromUserName = n.FromUserId.HasValue ? n.FromUser.FirstName + " " + n.FromUser.LastName : "",
                fromUserAvatar = n.FromUserId.HasValue ? n.FromUser.Avatar : "",
                newMissionTitle = n.NewMissionId.HasValue ? n.NewMission.Title : "",
                recommendedMissionTitle = n.RecommendedMissionId.HasValue ? n.RecommendedMission.Title : "",
                timesheetMissionTitle = n.TimesheetId.HasValue ? n.Timesheet.Mission.Title : "",
                commentMissionTitle = n.CommentId.HasValue ? n.Comment.Mission.Title: "",
                applicationMissionTitle = n.MissionApplicationId.HasValue ? n.MissionApplication.Mission.Title : "",
                approvedStoryTitle = n.StoryId.HasValue ? n.Story.Title : "",
                recommendedStoryTitle = n.RecommendedStooyId.HasValue ? n.RecommendedStooy.Title : "",
                recommendedStoryMissionId = n.RecommendedStooyId.HasValue ? n.RecommendedStooy.Mission.MissionId : 0,
                recommendedStoryUserId = n.RecommendedStooyId.HasValue ? n.RecommendedStooy.User.UserId : 0,
                timesheetApprovalStatus = n.TimesheetId.HasValue ? n.Timesheet.Status : "",
                commentApprovalStatus = n.CommentId.HasValue ? n.Comment.ApprovalStatus : "",
                applicationApprovalStatus = n.MissionApplicationId.HasValue ? n.MissionApplication.ApprovalStatus : "",
                storyapprovalStatus = n.StoryId.HasValue ? n.Story.Status : ""
            });

            return list.ToList();
        }
    }
}
