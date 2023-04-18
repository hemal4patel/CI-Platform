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
                missionId = m.MissionId,
                misssionTitle = m.Title,
                missionType = m.MissionType,
                startDate = m.StartDate,
                endDate = m.EndDate
            });

            return list.ToList();
        }

        public List<MissionTheme> GetThemes ()
        {
            return _db.MissionThemes.ToList();
        }

        public List<Skill> GetSkills ()
        {
            return _db.Skills.ToList();
        }

        public void DeleteMission (long missionId)
        {
            Mission mission = _db.Missions.FirstOrDefault(m => m.MissionId == missionId);
            mission.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

    }
}
