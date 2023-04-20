using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminMission
    {
        public List<AdminMissionList> GetMissions ();

        public List<MissionTheme> GetThemes ();

        public List<Skill> GetSkills ();

        public bool MissionExistsForNew (string title, string organizationName);

        public bool MissionExistsForUpdate (long? missionId, string title, string organizationName);

        public void AddMission (AdminMissionViewModel model);

        public AdminMissionList GetMissionToEdit (long missionId);

        //public void EditMission (AdminMissionViewModel model);

        public void DeleteMission (long missionId);
    }
}
