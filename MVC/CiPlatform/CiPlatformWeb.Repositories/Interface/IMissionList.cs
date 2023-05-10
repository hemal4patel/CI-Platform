using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IMissionList
    {

        public User sessionUser (long userId);

        public List<Country> GetCountryList ();

        public List<City> GetCityList (long countryId);

        public List<MissionTheme> GetThemeList ();

        public List<Skill> GetSkillList ();

        public (List<MissionListModel> missions, int count) GetMissions (MissionQueryParams viewmodel, long userId);

        public List<User> GetUserList (long userId);

        public MissionInvite HasAlreadyInvited (long ToUserId, long MissionId, long FromUserId);

        public Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link);

        public void AddToFavorites (long missionId, long userId);

        public void RateMission (long missionId, long userId, int rating);

        public List<MissionInvite> GetMissionInvites (long userId);

        public List<StoryInvite> GetStoryInvites (long userId);

        public List<NotificationParams> GetAllNotifications (long userId);

        public int ChangeNotificationStatus (long id);

        public void ClearAllNotifications (long userId);

        public long[] GetUserNotificationChanges(long userId);

        public void SaveUserNotificationChanges(long userId, long[] settingIds);
    }
}
