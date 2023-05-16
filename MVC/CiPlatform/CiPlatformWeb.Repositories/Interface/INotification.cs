using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface INotification
    {
        public List<NotificationParams> GetAllNotifications (long userId);

        public (List<NotificationParams> AllNotifications, int UnreadCount) GetUserNotifications (long userId);

        public int GetUnreadNotificationsCount (long userId);

        public int ChangeNotificationStatus (long id);

        public void ClearAllNotifications (long userId);

        public long[] GetUserNotificationChanges (long userId);

        public void SaveUserNotificationChanges (long userId, long[] settingIds);
    }
}
