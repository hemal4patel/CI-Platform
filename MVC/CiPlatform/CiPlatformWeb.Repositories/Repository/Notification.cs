using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class Notification : INotification
    {
        private readonly ApplicationDbContext _db;

        public Notification (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<NotificationParams> GetAllNotifications (long userId)
        {
            IQueryable<UserNotification> notifications = _db.UserNotifications.Where(u => u.ToUserId == userId && u.DeletedAt == null && u.UserSetting.IsEnabled == true).OrderByDescending(u => u.CreatedAt).AsQueryable();

            IQueryable<NotificationParams> list = notifications.Select(n => new NotificationParams()
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
                commentMissionTitle = n.CommentId.HasValue ? n.Comment.Mission.Title : "",
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

        public int GetUnreadNotificationsCount (long userId)
        {
            return _db.UserNotifications.Where(u => u.ToUserId == userId && u.DeletedAt == null && u.Status == false && u.UserSetting.IsEnabled == true).Count();
        }

        public int ChangeNotificationStatus (long id)
        {
            int flag = 1;
            UserNotification notification = _db.UserNotifications.FirstOrDefault(n => n.NotificationId == id);
            if (notification.Status == true)
            {
                flag = 0;
            }
            notification.Status = true;
            _db.SaveChanges();
            return flag;
        }

        public void ClearAllNotifications (long userId)
        {
            List<UserNotification> notifications = _db.UserNotifications.Where(u => u.ToUserId == userId).ToList();
            foreach (UserNotification n in notifications)
            {
                n.DeletedAt = DateTime.Now;
            }
            _db.SaveChanges();
        }

        public long[] GetUserNotificationChanges (long userId)
        {
            long[] settingIds = _db.UserSettings.Where(u => u.UserId == userId && u.IsEnabled == true).Select(u => u.SettingId).ToArray();
            return settingIds;
        }

        public void SaveUserNotificationChanges (long userId, long[] settingIds)
        {
            for (int i = 1; i <= 7; i++)
            {
                UserSetting setting = _db.UserSettings.FirstOrDefault(u => u.UserId == userId && u.SettingId == i);
                if (settingIds.Contains(setting.SettingId))
                {
                    setting.IsEnabled = true;
                }
                else
                {
                    setting.IsEnabled = false;
                }
            }
            _db.SaveChanges();
        }
    }
}
