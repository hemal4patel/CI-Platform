using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IMissionFunctions
    {
        public void AddComment (long missionId, long userId, string comment);
    }
}
