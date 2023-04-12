using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class DisplayMissionCards
    {
        public List<Country> CountryList { get; set; }

        public List<City> CityList { get; set; }

        public List<MissionTheme> ThemeList { get; set; }

        public List<Skill> SkillList { get; set; }

        public List<User> UserList { get; set; } = null;

        //to store data
        public List<Mission> MissionList { get; set; }

    }
}
