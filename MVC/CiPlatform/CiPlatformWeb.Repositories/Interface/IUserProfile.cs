using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IUserProfile
    {
        public User GetUserDetails (long userId);

        public List<Country> GetCountryList ();

        public List<City> GetCityList (long GetCityList);

        public List<Skill> GetSkillList ();
    }
}
