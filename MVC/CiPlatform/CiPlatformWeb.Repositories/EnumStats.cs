using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories
{
    public class EnumStats
    {
        //user notifications
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

        //story status
        public enum storyStatus
        {
            draft,
            pending,
            published, 
            declined
        }

        //mission application status
        public enum applicationStatus
        {
            pending,
            approve,
            decline
        }

        //comment status
        public enum commentStatus
        {
            pending,
            published,
            declined
        }

        //timesheet status
        public enum timesheetStatus
        {
            pending,
            approved,
            declined
        }

        //mission type
        public enum missionType
        {
            Time,
            Goal
        }

        //user role
        public enum userRole
        {
            user,
            admin
        }
    }
}
