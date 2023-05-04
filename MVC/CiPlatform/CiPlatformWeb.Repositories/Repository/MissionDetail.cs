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
            IQueryable<Mission> missions = _db.Missions.Where(m => m.MissionId == MissionId).AsQueryable();

            MissionDetailsModel? list = missions.Select(m => new MissionDetailsModel()
            {
                mission = m,
                missionId = m.MissionId,
                cityName = m.City.Name,
                themeName = m.Theme.Title,
                isFavorite = m.FavouriteMissions.Any(m => m.UserId == userId),
                ratingByUser = m.MissionRatings.Where(m => m.UserId == userId).Select(m => m.Rating).FirstOrDefault(),
                ratingByAll = m.MissionRatings.Any() ? m.MissionRatings.Average(m => m.Rating) : 0,
                ratedByVols = m.MissionRatings.Count(),
                seatsLeft = m.TotalSeats - m.MissionApplications.Where(m => m.ApprovalStatus == "APPROVE").Count(),
                hasDeadlinePassed = m.StartDate.Value.AddDays(-1) < DateTime.Now,
                haEndDatePassed = m.EndDate < DateTime.Now,
                isOngoing = (m.StartDate < DateTime.Now) && (m.EndDate > DateTime.Now),
                hasApplied = m.MissionApplications.Any(m => m.UserId == userId),
                goalObjectiveText = m.GoalMissions.Select(m => m.GoalObjectiveText).FirstOrDefault(),
                totalGoal = m.GoalMissions.Select(m => m.GoalValue).FirstOrDefault(),
                achievedGoal = m.Timesheets.Where(m => m.DeletedAt == null && m.Status == "APPROVED").Sum(m => m.Action),
                missionMedia = m.MissionMedia.Where(m => m.DeletedAt == null).ToList(),
                skills = m.MissionSkills.Where(m => m.DeletedAt == null).Select(m => m.Skill.SkillName).ToList(),
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
            MissionApplication missionApplication = new MissionApplication()
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

        public List<Comment> GetComments (long MissionId)
        {
            return _db.Comments.Where(c => c.DeletedAt == null && c.MissionId == MissionId).Include(c => c.User).OrderByDescending(c => c.CreatedAt).ToList();
        }

        public List<MissionListModel> GetRelatedMissions (long MissionId, long userId)
        {
            Mission mission = _db.Missions.Where(m => m.MissionId == MissionId).FirstOrDefault();

            IQueryable<Mission> relatedMissions = _db.Missions
                .Where(m => m.MissionId != MissionId && m.CityId == mission.CityId && m.DeletedAt == null && m.Status == 1)
                    .Take(3);

            if (relatedMissions.Count() < 3)
            {
                relatedMissions = relatedMissions.Union(
                    _db.Missions.Where(m => m.MissionId != MissionId && m.CountryId == mission.CountryId && m.DeletedAt == null && m.Status == 1)
                        .Take(3 - relatedMissions.Count()));
            }

            if (relatedMissions.Count() < 3)
            {
                relatedMissions = relatedMissions.Union(
                    _db.Missions.Where(m => m.MissionId != MissionId && m.ThemeId == mission.ThemeId && m.DeletedAt == null && m.Status == 1)
                        .Take(3 - relatedMissions.Count()));
            }

            IQueryable<MissionListModel> list = relatedMissions.Select(m => new MissionListModel()
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
                achievedGoal = m.Timesheets.Where(m => m.DeletedAt == null && m.Status == "APPROVED").Sum(m => m.Action),
                mediaPath = m.MissionMedia.Where(m => m.Default == 1 && m.DeletedAt == null).Select(m => m.MediaPath).FirstOrDefault(),
                skill = m.MissionSkills.Where(m => m.DeletedAt == null).Select(m => m.Skill.SkillName).FirstOrDefault()
            });

            return list.ToList();
        }

        public (List<MissionApplication> recentVolunteers, int count) GetRecentVolunteers (long MissionId, long userId, int pageno)
        {
            int pagesize = 2;
            IQueryable<MissionApplication> recentVolunteers = _db.MissionApplications
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
            Comment newComment = new Comment
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
            MissionInvite missionInvite = new MissionInvite()
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

        public (int ratings, int volunteers) GetUpdatedRatings (long missionId)
        {
            int ratings = (int) _db.MissionRatings.Where(r => r.MissionId == missionId).Average(r => r.Rating);
            int volunteers = _db.MissionRatings.Where(r => r.MissionId == missionId).Count();
            return (ratings, volunteers);
        }
    }
}
