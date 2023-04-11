﻿using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var timeMissions = _db.MissionApplications.Where(m => m.UserId == userId && m.Mission.MissionType == "Time" && m.ApprovalStatus == "APPROVE").Include(m => m.Mission).ToList();

            return timeMissions;
        }

        public List<MissionApplication> GetGoalBasedMission (long userId)
        {
            var goalMissions = _db.MissionApplications.Where(m => m.UserId == userId && m.Mission.MissionType == "Goal" && m.ApprovalStatus == "APPROVE").Include(m => m.Mission).ToList();

            return goalMissions;
        }

        public List<Timesheet> GetTimeBasedEntries (long userId)
        {
            var timeBasedEntries = _db.Timesheets.Where(t => t.UserId == userId && t.Mission.MissionType == "Time").Include(t => t.Mission).ToList();
            return timeBasedEntries;
        }

        public List<Timesheet> GetGoalBasedEntries (long userId)
        {
            var goalBasedEnteries = _db.Timesheets.Where(t => t.UserId == userId && t.Mission.MissionType == "Goal").Include(t => t.Mission).ToList();
            return goalBasedEnteries;
        }

        public Timesheet GetEntry (long? timesheetId)
        {
            var entry = _db.Timesheets.Where(t => t.TimesheetId == timesheetId).FirstOrDefault();
            return entry;
        }

        public void AddTimeBasedEntry (TimeBasedSheetViewModel viewmodel, long userId)
        {
            TimeSpan timeSpan = new TimeSpan(viewmodel.hours, viewmodel.minutes, 0);
            var timeBasedEntry = new Timesheet
            {
                UserId = userId,
                MissionId = viewmodel.timeMissions,
                Time = timeSpan,
                Action = null,
                DateVolunteered = viewmodel.dateVolunteered,
                Notes = viewmodel.message,
                CreatedAt = DateTime.Now
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

            _db.Update(timeBasedEntry);
            _db.SaveChanges();
        }

        public void AddGoalBasedEntry (GoalBasedSheetViewModel viewmodel, long userId)
        {
            var goalBasedEntry = new Timesheet
            {
                UserId = userId,
                MissionId = viewmodel.goalMissions,
                Time = null,
                Action = viewmodel.actions,
                DateVolunteered = viewmodel.dateVolunteered,
                Notes = viewmodel.message,
                CreatedAt = DateTime.Now
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

            _db.Update(goalBasedEntry);
            _db.SaveChanges();
        }

        public void DeleteTimesheetEntry (long id)
        {
            var timesheet = GetEntry(id);
            _db.Timesheets.Remove(timesheet);
            _db.SaveChanges();
        }
    }
}