using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class MissionDetail : IMissionDetail
    {
        private readonly ApplicationDbContext _db;

        public MissionDetail (ApplicationDbContext db)
        {
            _db = db;
        }

        public MissionDetailsModel GetMissionDetails (long MissionId, long userId)
        {
            var missions = _db.Missions.Where(m => m.MissionId == MissionId).AsQueryable();

            var list = missions.Select(m => new MissionDetailsModel()
            {
                mission = m,
                missionId = m.MissionId,
                cityName = m.City.Name,
                themeName = m.Theme.Title,
                isFavorite = m.FavouriteMissions.Any(m => m.UserId == userId),
                rating = m.MissionRatings.Select(m => m.Rating).FirstOrDefault(),
                ratedByVols = m.MissionRatings.Count(),
                seatsLeft = m.TotalSeats - m.MissionApplications.Where(m => m.ApprovalStatus == "APPROVE").Count(),
                hasDeadlinePassed = m.StartDate.Value.AddDays(-1) < DateTime.Now,
                haEndDatePassed = m.EndDate < DateTime.Now,
                isOngoing = (m.StartDate < DateTime.Now) && (m.EndDate > DateTime.Now),
                hasApplied = m.MissionApplications.Any(m => m.UserId == userId),
                goalObjectiveText = m.GoalMissions.Select(m => m.GoalObjectiveText).FirstOrDefault(),
                totalGoal = m.GoalMissions.Select(m => m.GoalValue).FirstOrDefault(),
                achievedGoal = m.Timesheets.Sum(m => m.Action),
                missionMedia = m.MissionMedia.Where(m => m.DeletedAt == null).ToList(),
                skills = m.MissionSkills.Select(m => m.Skill.SkillName).ToList(),
                ApprovedComments = m.Comments.ToList(),
                MissionDocuments = m.MissionDocuments.Where(m => m.DeletedAt == null).ToList(),
                hasAppliedApprove = m.MissionApplications.Any(m => m.UserId == userId && m.ApprovalStatus == "APPROVE"),
                hasAppliedPending = m.MissionApplications.Any(m => m.UserId == userId && m.ApprovalStatus == "PENDING"),
                hasAppliedDecline = m.MissionApplications.Any(m => m.UserId == userId && m.ApprovalStatus == "DECLINE")
            }).FirstOrDefault();

            return list;
        }

        public bool HasAlreadyApplied (long missionId, long userId)
        {
            return _db.MissionApplications.Any(ma => ma.MissionId == missionId && ma.UserId == userId);
        }

        public void ApplyToMission (long missionId, long userId)
        {
            var missionApplication = new MissionApplication()
            {
                MissionId = missionId,
                UserId = userId,
                ApprovalStatus = "PENDING",
                CreatedAt = DateTime.Now,
                AppliedAt = DateTime.Now
            };
            _db.MissionApplications.Add(missionApplication);
            _db.SaveChanges();
        }

        public List<MissionListModel> GetRelatedMissions (long MissionId, long userId)
        {
            var mission = _db.Missions.Where(m => m.MissionId == MissionId).FirstOrDefault();

            var relatedMissions = _db.Missions
                .Where(m => m.MissionId != MissionId && m.CityId == mission.CityId)
                    .Take(3);

            if (relatedMissions.Count() < 3)
            {
                relatedMissions = relatedMissions.Union(
                    _db.Missions.Where(m => m.MissionId != MissionId && m.CountryId == mission.CountryId)
                        .Take(3 - relatedMissions.Count()));
            }

            if (relatedMissions.Count() < 3)
            {
                relatedMissions = relatedMissions.Union(
                    _db.Missions.Where(m => m.MissionId != MissionId && m.ThemeId == mission.ThemeId)
                        .Take(3 - relatedMissions.Count()));
            }

            var list = relatedMissions.Select(m => new MissionListModel()
            {
                mission = m,
                missionId = m.MissionId,
                cityName = m.City.Name,
                themeName = m.Theme.Title,
                isFavorite = m.FavouriteMissions.Any(m => m.UserId == userId),
                rating = m.MissionRatings.Select(m => m.Rating).FirstOrDefault(),
                seatsLeft = m.TotalSeats - m.MissionApplications.Where(m => m.ApprovalStatus == "APPROVE").Count(),
                hasDeadlinePassed = m.StartDate.Value.AddDays(-1) < DateTime.Now,
                haEndDatePassed = m.EndDate < DateTime.Now,
                isOngoing = (m.StartDate < DateTime.Now) && (m.EndDate > DateTime.Now),
                hasApplied = m.MissionApplications.Any(m => m.UserId == userId),
                goalObjectiveText = m.GoalMissions.Select(m => m.GoalObjectiveText).FirstOrDefault(),
                totalGoal = m.GoalMissions.Select(m => m.GoalValue).FirstOrDefault(),
                achievedGoal = m.Timesheets.Sum(m => m.Action),
                mediaPath = m.MissionMedia.Where(m => m.Default == 1).Select(m => m.MediaPath).FirstOrDefault(),
                skill = m.MissionSkills.Select(m => m.Skill.SkillName).FirstOrDefault()
            });

            return list.ToList();
        }

        public (List<MissionApplication> recentVolunteers, int count) GetRecentVolunteers (long MissionId, long userId, int pageno)
        {
            var pagesize = 2;
            var recentVolunteers = _db.MissionApplications
                .Include(u => u.User)
                .Where(u => u.MissionId == MissionId && u.UserId != userId && u.ApprovalStatus == "APPROVE");

            int count = recentVolunteers.Count();

            recentVolunteers = _db.MissionApplications
                .Include(u => u.User)
                .Where(u => u.MissionId == MissionId && u.UserId != userId && u.ApprovalStatus == "APPROVE").OrderByDescending(u => u.CreatedAt)
                .Skip(Math.Max((pageno - 1) * pagesize, 0))
                .Take(pagesize);

            return (recentVolunteers.ToList(), count);
        }

        public void AddComment (long missionId, long userId, string comment)
        {
            var newComment = new Comment
            {
                MissionId = missionId,
                UserId = userId,
                CommentText = comment,
                CreatedAt = DateTime.Now
            };
            _db.Comments.Add(newComment);
            _db.SaveChanges();
        }

        public void InviteToMission (long FromUserId, long ToUserId, long MissionId)
        {
            var missionInvite = new MissionInvite()
            {
                FromUserId = FromUserId,
                ToUserId = ToUserId,
                MissionId = MissionId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _db.MissionInvites.Add(missionInvite);
            _db.SaveChanges();
        }

        public void ReInviteToMission (MissionInvite MissionInvite)
        {
            MissionInvite.UpdatedAt = DateTime.Now;
            _db.Update(MissionInvite);
            _db.SaveChanges();
        }
    }
}
