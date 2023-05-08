using System;
using System.Collections.Generic;

namespace CiPlatformWeb.Entities.DataModels;

public partial class NotificationSetting
{
    public long SettingId { get; set; }

    public string SettingName { get; set; } = null!;

    public bool? IsEnabled { get; set; }
}
