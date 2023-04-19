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

        public AdminMissionList GetMissionToEdit (long missionId)
        {
            var mission = _db.Missions.Where(m => m.MissionId == missionId);

            var list = mission.Select(m => new AdminMissionList()
            {
                missionId = m.MissionId,
                misssionTitle = m.Title,
                shortDescription = m.ShortDescription,
                missionDescription = m.Description,
                countryId = m.CountryId,
                cityId = m.CityId,
                organizationName = m.OrganizationName,
                organizationDetail = m.OrganizationDetail,
                startDate = m.StartDate,
                endDate = m.EndDate,
                missionType = m.MissionType,
                totalSeats = m.TotalSeats,
                goalObjectiveText = m.GoalMissions.Select(m => m.GoalObjectiveText).FirstOrDefault(),
                goalValue = m.GoalMissions.Select(m => m.GoalValue).FirstOrDefault(),
                missionTheme = m.Theme.MissionThemeId,
                missionSkills = string.Join(",", m.MissionSkills.Select(m => m.Skill.SkillId)),
                availability = m.Availability,
                imageName = string.Join(",", m.MissionMedia.Where(m => m.MediaType == "img").Select(m => $"{m.MediaPath}:{m.Default}"))
            }).FirstOrDefault();

            return list;
        }

        public void DeleteMission (long missionId)
        {
            Mission mission = _db.Missions.FirstOrDefault(m => m.MissionId == missionId);
            mission.DeletedAt = DateTime.Now;
            _db.SaveChanges();
        }

    }
}
