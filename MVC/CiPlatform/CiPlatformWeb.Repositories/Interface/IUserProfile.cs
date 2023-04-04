using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IUserProfile
    {
        public UserProfileViewModel GetUserDetails (long userId);

        public List<Country> GetCountryList ();

        public List<City> GetCityList (long GetCityList);

        public List<Skill> GetSkillList ();

        public User CheckPassword (long userId, string oldPassoword);

        public void UpdatePassword (User user, string newPassoword);
    }
}
