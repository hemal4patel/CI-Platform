using CiPlatformWeb.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class SessionUserViewModel
    {
        public long userId { get; set; }

        public string userName { get; set; }

        public string emailId { get; set; }

        public string? avatarName { get; set; }

        public List<CmsPage> cmsPages { get; set; }
    }
}
