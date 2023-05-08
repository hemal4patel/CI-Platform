using System;
using System.Collections.Generic;

namespace CiPlatformWeb.Entities.DataModels;

public partial class UserNotification
{
    public long NotificationId { get; set; }

    public long ToUserId { get; set; }

    public long? FromUserId { get; set; }

    public long? RecommendedMissionId { get; set; }

    public long? TimesheetId { get; set; }

    public long? CommentId { get; set; }

    public long? StoryId { get; set; }

    public long? NewMissionId { get; set; }

    public long? RecommendedStooyId { get; set; }

    public long? MissionApplicationId { get; set; }

    public bool Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long UserSettingId { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual MissionApplication? MissionApplication { get; set; }

    public virtual Mission? NewMission { get; set; }

    public virtual Mission? RecommendedMission { get; set; }

    public virtual Story? RecommendedStooy { get; set; }

    public virtual Story? Story { get; set; }

    public virtual Timesheet? Timesheet { get; set; }

    public virtual User ToUser { get; set; } = null!;

    public virtual UserSetting UserSetting { get; set; } = null!;
}
