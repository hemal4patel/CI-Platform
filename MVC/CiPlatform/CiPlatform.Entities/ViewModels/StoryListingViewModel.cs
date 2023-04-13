using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class StoryListingViewModel
    {
        public List<Country> CountryList { get; set; }

        public List<City> CityList { get; set; }

        public List<MissionTheme> ThemeList { get; set; }

        public List<Skill> SkillList { get; set; }

        //to store data
        public List<StoryListModel> StoryList { get; set; }

        public int StoryCount { get; set; }

    }

    public class StoryListModel
    {
        public Story story { get; set; }

        public string? mediaPath { get; set; }

        public string themeName { get; set; }

        public string? storyUserAvatar { get; set; }

        public string storyUserName { get; set; }
    }
}
