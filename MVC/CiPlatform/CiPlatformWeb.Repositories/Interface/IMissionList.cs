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
        public List<Country> GetCountryList ();

        public List<City> GetCityList (long countryId);

        public List<MissionTheme> GetThemeList ();

        public List<Skill> GetSkillList ();

        public (List<Mission> missions, int count) GetMissions (DisplayMissionCards viewmodel, long userId);

        public List<User> GetUserList (long userId);

        public MissionInvite HasAlreadyInvited (long ToUserId, long MissionId, long FromUserId);

        public Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link);
    }
}
