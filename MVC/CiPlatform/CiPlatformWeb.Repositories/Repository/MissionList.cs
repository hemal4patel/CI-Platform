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
    public class MissionList : IMissionList
    {
        private readonly ApplicationDbContext _db;

        public MissionList (ApplicationDbContext db)
        {
            _db = db;
        }
        public IEnumerable<Mission> GetMissions (List<long> MissionIds)
        {
            var MissionList = _db.Missions.Where(m => MissionIds.Contains(m.MissionId))
                    .Include(m => m.City)
                    .Include(m => m.Country)
                    .Include(m => m.MissionSkills).ThenInclude(ms => ms.Skill)
                    .Include(m => m.Theme)
                    .Include(m => m.MissionRatings)
                    .Include(m => m.GoalMissions)
                    .Include(m => m.MissionApplications)
                    .Include(m => m.FavouriteMissions)
                    .Include(m => m.MissionMedia).ToList().OrderBy(ml => MissionIds.IndexOf(ml.MissionId));

            return MissionList;


        }
    }
}
