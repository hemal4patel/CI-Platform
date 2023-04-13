using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminUserViewModel
    {
        public List<AdminUserModel> users { get; set; }
    }

    public class AdminUserModel
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public string? employeeId { get; set; }

        public string? department { get; set; }

        public int status { get; set; }
    }
}
