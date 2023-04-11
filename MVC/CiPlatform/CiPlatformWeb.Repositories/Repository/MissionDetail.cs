using CiPlatformWeb.Entities.DataModels;
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

        public Mission GetMissionDetails (long MissionId)
        {
            Mission mission = _db.Missions
                .Include(m => m.Country)
                .Include(m => m.City)
                .Include(m => m.MissionRatings)
                .Include(m => m.Theme)
                .Include(m => m.MissionSkills).ThenInclude(m => m.Skill)
                .Include(m => m.MissionApplications)
                .Include(m => m.GoalMissions)
                .Include(m => m.FavouriteMissions)
                .Include(m => m.MissionMedia)
                .Include(m => m.Timesheets)
                .FirstOrDefault(m => m.MissionId == MissionId);

            return mission;
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


        public List<Comment> GetApprovedComments (long MissionId)
        {
            var approvedComments = _db.Comments.Where(c => c.MissionId == MissionId && c.ApprovalStatus == "PUBLISHED")
                .Include(c => c.User).OrderByDescending(c => c.CreatedAt).ToList();

            return approvedComments;
        }

        public List<Mission> GetRelatedMissions (long MissionId)
        {
            var mission = _db.Missions.Where(m => m.MissionId == MissionId).FirstOrDefault();

            var relatedMissions = new List<Mission>();

            relatedMissions.AddRange(_db.Missions.Where(m => m.MissionId != MissionId && m.CityId == mission.CityId)
                .Include(m => m.Country)
                .Include(m => m.City)
                .Include(m => m.MissionRatings)
                .Include(m => m.Theme)
                .Include(m => m.MissionSkills).ThenInclude(m => m.Skill)
                .Include(m => m.MissionApplications)
                .Include(m => m.GoalMissions)
                .Include(m => m.FavouriteMissions)
                .Include(m => m.MissionMedia).Take(3));

            if (relatedMissions.Count < 3)
            {
                relatedMissions.AddRange(_db.Missions.Where(m => m.MissionId != MissionId && m.CountryId == mission.CountryId)
                .Include(m => m.Country)
                .Include(m => m.City)
                .Include(m => m.MissionRatings)
                .Include(m => m.Theme)
                .Include(m => m.MissionSkills).ThenInclude(m => m.Skill)
                .Include(m => m.MissionApplications)
                .Include(m => m.GoalMissions)
                .Include(m => m.FavouriteMissions)
                .Include(m => m.MissionMedia).Take(3 - relatedMissions.Count));
            }

            if (relatedMissions.Count < 3)
            {
                relatedMissions.AddRange(_db.Missions.Where(m => m.MissionId != MissionId && m.ThemeId == mission.ThemeId)
                .Include(m => m.Country)
                .Include(m => m.City)
                .Include(m => m.MissionRatings)
                .Include(m => m.Theme)
                .Include(m => m.MissionSkills).ThenInclude(m => m.Skill)
                .Include(m => m.MissionApplications)
                .Include(m => m.GoalMissions)
                .Include(m => m.FavouriteMissions)
                .Include(m => m.MissionMedia).Take(3 - relatedMissions.Count));
            }

            return relatedMissions;
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


        public List<MissionDocument> GetMissionDocuments (long MissionId)
        {
            return _db.MissionDocuments.Where(m => m.MissionId == MissionId).ToList();
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
