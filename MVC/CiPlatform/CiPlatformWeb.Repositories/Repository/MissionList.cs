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
        public IEnumerable<Mission> GetMissions ()
        {
            //IEnumerable<Mission> MissionListTime = _db.Missions.Where(m => m.MissionType.Equals("Time")).Include(m => m.City).Include(m => m.Country).Include(m => m.MissionSkills).Include(m => m.Theme).Include(m => m.MissionRatings).Include(m => m.GoalMissions);

            //IEnumerable<Mission> MissionList = _db.Missions.Where(m => m.MissionType.Equals("Goal")).Include(m => m.City).Include(m => m.Country).Include(m => m.MissionSkills).Include(m => m.Theme).Include(m => m.MissionRatings).Include(m => m.GoalMissions);

            IEnumerable<Mission> MissionList = _db.Missions.Include(m => m.City).Include(m => m.Country).Include(m => m.MissionSkills).Include(m => m.Theme).Include(m => m.MissionRatings).Include(m => m.GoalMissions);
            return MissionList;
        }
    }
}
