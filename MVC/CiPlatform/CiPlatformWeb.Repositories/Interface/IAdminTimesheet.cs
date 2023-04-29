using CiPlatformWeb.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Interface
{
    public interface IAdminTimesheet
    {
        public List<AdminTimesheetModel> GetTimesheets();

        public void ChangeTimesheetStatus (long timesheetId, int status);
    }
}
