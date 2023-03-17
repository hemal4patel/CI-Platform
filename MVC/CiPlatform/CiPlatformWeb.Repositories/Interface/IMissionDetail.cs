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

        public List<Comment> GetApprovedComments (long MissionId);

        public List<Mission> GetRelatedMissions (long MissionId);

        public List<MissionApplication> GetRecentVolunteers (long MissionId, long userId);
    }
}
