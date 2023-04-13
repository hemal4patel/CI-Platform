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
        public List<MissionListModel> MissionList { get; set; }

        public int MissionCount { get; set; }

    }

    public class MissionListModel
    {
        public Mission mission { get; set; }

        public long missionId { get; set; }

        public string cityName { get; set; }

        public string themeName { get; set; }

        public bool isFavorite { get; set; }

        public double rating { get; set; }

        public int? seatsLeft { get; set; }

        public bool hasDeadlinePassed { get; set; }

        public bool haEndDatePassed { get; set; }

        public bool isOngoing { get; set; }

        public bool hasApplied { get; set; }

        public string? goalObjectiveText { get; set; }

        public int? totalGoal { get; set; }

        public int? achievedGoal { get; set; }

        public string? mediaPath { get; set; }

        public string? skill { get; set; }
    }
}
