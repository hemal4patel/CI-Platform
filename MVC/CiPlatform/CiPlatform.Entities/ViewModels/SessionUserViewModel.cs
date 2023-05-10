using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class SessionUserViewModel
    {
        public long userId { get; set; }

        public string userName { get; set; }

        public string emailId { get; set; }

        public string? avatarName { get; set; }

        public string role { get; set; }

        public List<CmsPage> cmsPages { get; set; }

        public CmsPage selectedCmsPage { get; set; }

        public List<NotificationParams> userNotifications { get; set; }
    }

    public class NotificationModel
    {
        public List<NotificationParams> userNotifications { get; set; }

        public int UnreadCount { get; set; }
    }

    public class NotificationParams
    {
        public long notificationId { get; set; }

        public long toUserId { get; set; }

        public long? fromUserId { get; set; }

        public long? recommendedMissionId { get; set; }

        public long? recommendedStoryId { get; set; }

        public long? newMissionId { get; set; }

        public long? storyId { get; set; }

        public long? timesheetId { get; set; }

        public long? commentId { get; set; }

        public long? MissionApplicationId { get; set; }

        public bool status { get; set; }

        public DateTime? createdAt { get; set; }

        public string? fromUserName { get; set; }

        public string? fromUserAvatar{ get; set; }

        public string? newMissionTitle { get; set; }
        public string? recommendedMissionTitle { get; set; }
        public string? timesheetMissionTitle { get; set; }
        public string? commentMissionTitle { get; set; }
        public string? applicationMissionTitle { get; set; }

        public string? approvedStoryTitle { get; set; }
        public string? recommendedStoryTitle { get; set; }
        public long? recommendedStoryMissionId { get; set; }
        public long? recommendedStoryUserId { get; set; }

        public string? timesheetApprovalStatus { get; set; }
        public string? commentApprovalStatus { get; set; }
        public string? applicationApprovalStatus { get; set; }
        public string? storyapprovalStatus { get; set; }
    }
}
