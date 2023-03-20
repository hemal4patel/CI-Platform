using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IMissionList
    {
        public IEnumerable<Mission> GetMissions (List<long> MissionIds);

        public List<User> UserList (long userId);

        public Task SendInvitationToCoWorker (long ToUserId, long FromUserId, DisplayMissionCards viewmodel);
    }
}
