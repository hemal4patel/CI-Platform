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

        public List<Story> StoryList { get; set; }

        //linq filter params
        public string? searchText { get; set; }

        public long? CountryId { get; set; }

        public long[]? CityId { get; set; }

        public long[]? ThemeId { get; set; }

        public long[]? SkillId { get; set; }

        public int? sortCase { get; set; }

        public int pageNo { get; set; }
    }
}
