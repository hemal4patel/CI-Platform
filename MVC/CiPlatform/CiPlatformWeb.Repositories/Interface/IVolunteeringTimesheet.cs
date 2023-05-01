using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IVolunteeringTimesheet
    {
        List<MissionApplication> GetTimeBasedMission (long userId);

        List<MissionApplication> GetGoalBasedMission (long userId);

        List<Timesheet> GetTimeBasedEntries (long userId);

        List<Timesheet> GetGoalBasedEntries (long userId);

        public bool TimeSheetExists (long missionId, long userId, DateTime dateVolunteered);

        public bool TimesheetExistsForUpdate (long missionId, long userId, DateTime dateVolunteered, long? timesheetId);

        Timesheet GetEntry (long? timesheetId);

        void AddTimeBasedEntry (TimeBasedSheetViewModel viewmodel, long userId);

        void UpdateTimeBasedEntry (Timesheet timeBasedEntry, TimeBasedSheetViewModel viewmodel);

        void AddGoalBasedEntry (GoalBasedSheetViewModel viewmodel, long userId);

        void UpdateGoalBasedEntry (Timesheet goalBasedEntry, GoalBasedSheetViewModel viewmodel);

        void DeleteTimesheetEntry (long id);
    }
}
