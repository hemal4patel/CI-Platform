﻿using CiPlatformWeb.Entities.DataModels;
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

        public List<MissionApplication> Application { get; set; } = null;

        public List<MissionRating> MissionRatings { get; set; } = null;

        public List<GoalMission> GoalMissions { get; set; } = null;

        public List<User> UserList { get; set; } = null;

        public string? link { get; set; }

        //to store data
        public IEnumerable<Mission> MissionList { get; set; }


        // invites 

        public List<MissionInvite>? missionInvites { get; set; }

        public List<StoryInvite>? storyInvites { get; set; }

        public List<baseClass>? combinedList { get; set; }
    }
}
