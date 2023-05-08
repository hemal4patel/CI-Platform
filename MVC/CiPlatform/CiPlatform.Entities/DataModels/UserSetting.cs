using System;
using System.Collections.Generic;

namespace CiPlatformWeb.Entities.DataModels;

public partial class UserSetting
{
    public long UserSettingId { get; set; }

    public long SettingId { get; set; }

    public long UserId { get; set; }

    public bool? IsEnabled { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; } = new List<UserNotification>();
}
