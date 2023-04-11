using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IMissionDetail
    {
        public Mission GetMissionDetails (long MissionId);

        public bool HasAlreadyApplied (long missionId, long userId);

        public void ApplyToMission (long missionId, long userId);
        
        public List<Comment> GetApprovedComments (long MissionId);

        public List<Mission> GetRelatedMissions (long MissionId);

        public (List<MissionApplication> recentVolunteers, int count) GetRecentVolunteers (long MissionId, long userId, int pageno);

        public List<MissionDocument> GetMissionDocuments (long MissionId);

        public void AddComment (long MissionId, long userId, string comment);

        public void InviteToMission (long FromUserId, long ToUserId, long MissionId);

        public void ReInviteToMission (MissionInvite MissionInvite);
    }
}
