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
            Mission mission = _db.Missions.Include(m => m.Country).Include(m => m.City).Include(m => m.MissionRatings).Include(m => m.Theme).Include(m => m.MissionSkills).ThenInclude(m => m.Skill).Include(m => m.MissionApplications).Include(m => m.GoalMissions).Include(m => m.FavouriteMissions).Include(m => m.MissionMedia).Include(m => m.Comments).ThenInclude(u => u.User).FirstOrDefault(m => m.MissionId == MissionId);

            return mission;
        }
    }
}
