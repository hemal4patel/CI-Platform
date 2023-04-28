using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.ViewModels
{
    public class AdminCommentViewModel
    {
        public List<AdminCommentModel> comments { get; set; }
    }

    public class AdminCommentModel
    {
        public long commentId { get; set; }

        public string comment { get; set; }

        public string mission { get; set; }

        public string user { get; set; }

        public string status { get; set; }
    }
}
