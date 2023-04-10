using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class VolunteeringTimesheetViewModel
    {
        public List<MissionApplication> timeMissions { get; set; }

        public List<Timesheet> timeBasedEntries { get; set; }

        public List<MissionApplication> goalMissions { get; set; }

        public List<Timesheet> goalBasedEnteries { get; set; }

        public TimeBasedSheetViewModel timeBasedSheet { get; set; }

        public GoalBasedSheetViewModel goalBasedSheet { get; set; }
    }
}
