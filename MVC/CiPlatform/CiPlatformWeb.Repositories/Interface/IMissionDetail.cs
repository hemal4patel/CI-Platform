using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IMissionDetail
    {
        public MissionDetailsModel GetMissionDetails (long MissionId, long userId);

        public bool HasAlreadyApplied (long missionId, long userId);

        public void ApplyToMission (long missionId, long userId);

        public List<Comment> GetComments (long MissionId);

        public List<MissionListModel> GetRelatedMissions (long MissionId, long userId);

        public (List<MissionApplication> recentVolunteers, int count) GetRecentVolunteers (long MissionId, long userId, int pageno);

        public void AddComment (long MissionId, long userId, string comment);

        public void InviteToMission (long FromUserId, long ToUserId, long MissionId);

        public void ReInviteToMission (MissionInvite MissionInvite);
    }
}
