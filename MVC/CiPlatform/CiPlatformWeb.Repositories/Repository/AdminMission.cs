using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class AdminMission : IAdminMission
    {
        private readonly ApplicationDbContext _db;

        public AdminMission (ApplicationDbContext db)
        {
            _db = db;   
        }

        public List<AdminMissionList> GetMissions ()
        {
            IQueryable<Mission> missions = _db.Missions.Where(m => m.DeletedAt == null).AsQueryable();

            IQueryable<AdminMissionList> list = missions.Select(m => new AdminMissionList()
            {
                misssionTitle = m.Title,
                missionType = m.MissionType,
                startDate = m.StartDate.Value.ToShortDateString(),
                endDate = m.EndDate.Value.ToShortDateString()
            });

            return list.ToList();
        }
    }
}
