using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .FirstOrDefault(m => m.MissionId == MissionId);

            return mission;
        }

        public List<Comment> GetApprovedComments (long MissionId)
        {
            var approvedComments = _db.Comments.Where(c => c.MissionId == MissionId && c.ApprovalStatus == "PUBLISHED")
                .Include(c => c.User).ToList();

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

            if(relatedMissions.Count < 3)
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

        public List<MissionApplication> GetRecentVolunteers(long MissionId, long userId)
        {
            var recentVolunteers = _db.MissionApplications.Include(u => u.User).Where(u => u.MissionId == MissionId && u.UserId != userId).OrderByDescending(u => u.CreatedAt).ToList();

                return recentVolunteers;
        }
    }
}
