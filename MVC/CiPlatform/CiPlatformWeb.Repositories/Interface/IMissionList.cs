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

        public IEnumerable<Mission> GetMissions (List<long> MissionIds);

        public List<User> GetUserList (long userId);

        public Task SendInvitationToCoWorker (long ToUserId, long FromUserId, string link);
    }
}
