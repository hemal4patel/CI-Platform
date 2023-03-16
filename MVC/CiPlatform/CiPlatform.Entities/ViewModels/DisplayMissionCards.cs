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
        public List<Country> Country { get; set; } = null;

        public List<City> City { get; set; } = null;

        public List<MissionTheme> Theme { get; set; } = null;

        public List<Skill> Skill { get; set; } = null;

        public List<MissionApplication> Application { get; set; } = null;

        public List<MissionRating> MissionRatings { get; set; } = null;

        public List<GoalMission> GoalMissions { get; set; } = null;

        //to store data
        public IEnumerable<Mission> MissionList { get; set; }
    }
}
