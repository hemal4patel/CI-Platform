using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories
{
    public class EnumStats
    {
        public enum notifications
        {
            recommendedMission = 1,
            recommendedStory = 2,
            timesheet = 3,
            story = 4,
            missionApplication = 5,
            newMission = 6,
            comment = 7
        }
    }
}
