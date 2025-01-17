﻿using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CiPlatformWeb.Repositories.EnumStats;

namespace CiPlatformWeb.Repositories.Repository
{
    public class VolunteeringTimesheet : IVolunteeringTimesheet
    {
        private readonly ApplicationDbContext _db;

        public VolunteeringTimesheet (ApplicationDbContext db)
        {
            _db = db;
        }

        public List<MissionApplication> GetTimeBasedMission (long userId)
        {
            DateTime today = DateTime.Now;
            List<MissionApplication> timeMissions = _db.MissionApplications.Where(m => m.UserId == userId && m.Mission.MissionType == missionType.Time.ToString() && m.ApprovalStatus == applicationStatus.approve.ToString().ToUpper() && m.Mission.StartDate <= today && m.Mission.EndDate >= today).Include(m => m.Mission).ToList();

            return timeMissions;
        }

        public List<MissionApplication> GetGoalBasedMission (long userId)
        {
            DateTime today = DateTime.Now;
            List<MissionApplication> goalMissions = _db.MissionApplications.Where(m => m.UserId == userId && m.Mission.MissionType == missionType.Goal.ToString() && m.ApprovalStatus == applicationStatus.approve.ToString().ToUpper() && m.Mission.StartDate <= today && m.Mission.EndDate >= today).Include(m => m.Mission).ToList();

            return goalMissions;
        }

        public List<Timesheet> GetTimeBasedEntries (long userId)
        {
            List<Timesheet> timeBasedEntries = _db.Timesheets.Where(t => t.UserId == userId && t.Mission.MissionType == missionType.Time.ToString() && t.Status != timesheetStatus.declined.ToString().ToUpper() && t.DeletedAt == null).Include(t => t.Mission).ToList();
            return timeBasedEntries;
        }

        public List<Timesheet> GetGoalBasedEntries (long userId)
        {
            List<Timesheet> goalBasedEnteries = _db.Timesheets.Where(t => t.UserId == userId && t.Mission.MissionType == missionType.Goal.ToString() && t.Status != timesheetStatus.declined.ToString().ToUpper() && t.DeletedAt == null).Include(t => t.Mission).ToList();
            return goalBasedEnteries;
        }

        public bool TimeSheetExists (long missionId, long userId, DateTime dateVolunteered)
        {
            return _db.Timesheets.Any(t => t.MissionId == missionId && t.UserId == userId && t.DateVolunteered == dateVolunteered && t.Status != timesheetStatus.declined.ToString().ToUpper() && t.DeletedAt == null);
        }

        public bool TimesheetExistsForUpdate (long missionId, long userId, DateTime dateVolunteered, long? timesheetId)
        {
            return _db.Timesheets.Any(t => t.MissionId == missionId && t.UserId == userId && t.DateVolunteered == dateVolunteered && t.Status != timesheetStatus.declined.ToString().ToUpper() && t.DeletedAt == null && t.TimesheetId != timesheetId);
        }

        public Timesheet GetEntry (long? timesheetId)
        {
            Timesheet entry = _db.Timesheets.Where(t => t.TimesheetId == timesheetId).FirstOrDefault();
            return entry;
        }

        public void AddTimeBasedEntry (TimeBasedSheetViewModel viewmodel, long userId)
        {
            TimeSpan timeSpan = new TimeSpan(viewmodel.hours, viewmodel.minutes, 0);
            Timesheet timeBasedEntry = new Timesheet
            {
                UserId = userId,
                MissionId = viewmodel.timeMissions,
                Time = timeSpan,
                Action = null,
                DateVolunteered = viewmodel.dateVolunteered,
                Notes = viewmodel.message,
                CreatedAt = DateTime.Now,
                Status = timesheetStatus.pending.ToString().ToUpper()
            };
            _db.Timesheets.Add(timeBasedEntry);
            _db.SaveChanges();
        }

        public void UpdateTimeBasedEntry (Timesheet timeBasedEntry, TimeBasedSheetViewModel viewmodel)
        {
            TimeSpan timeSpan = new TimeSpan(viewmodel.hours, viewmodel.minutes, 0);
            timeBasedEntry.MissionId = viewmodel.timeMissions;
            timeBasedEntry.Time = timeSpan;
            timeBasedEntry.Action = null;
            timeBasedEntry.DateVolunteered = viewmodel.dateVolunteered;
            timeBasedEntry.Notes = viewmodel.message;
            timeBasedEntry.UpdatedAt = DateTime.Now;
            timeBasedEntry.Status = timesheetStatus.pending.ToString().ToUpper();

            _db.Update(timeBasedEntry);
            _db.SaveChanges();
        }

        public void AddGoalBasedEntry (GoalBasedSheetViewModel viewmodel, long userId)
        {
            Timesheet goalBasedEntry = new Timesheet
            {
                UserId = userId,
                MissionId = viewmodel.goalMissions,
                Time = null,
                Action = viewmodel.actions,
                DateVolunteered = viewmodel.dateVolunteered,
                Notes = viewmodel.message,
                CreatedAt = DateTime.Now,
                Status = timesheetStatus.pending.ToString().ToUpper()
            };
            _db.Timesheets.Add(goalBasedEntry);
            _db.SaveChanges();
        }

        public void UpdateGoalBasedEntry (Timesheet goalBasedEntry, GoalBasedSheetViewModel viewmodel)
        {
            goalBasedEntry.MissionId = viewmodel.goalMissions;
            goalBasedEntry.Time = null;
            goalBasedEntry.Action = viewmodel.actions;
            goalBasedEntry.DateVolunteered = viewmodel.dateVolunteered;
            goalBasedEntry.Notes = viewmodel.message;
            goalBasedEntry.UpdatedAt = DateTime.Now;
            goalBasedEntry.Status = timesheetStatus.pending.ToString().ToUpper();
            _db.Update(goalBasedEntry);
            _db.SaveChanges();
        }

        public void DeleteTimesheetEntry (long id)
        {
            Timesheet timesheet = GetEntry(id);
            timesheet.DeletedAt= DateTime.Now;
            _db.SaveChanges();
        }
    }
}
