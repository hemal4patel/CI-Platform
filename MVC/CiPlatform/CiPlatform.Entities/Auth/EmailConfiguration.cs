using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.Auth
{
    public class EmailConfiguration
    {
        public string fromEmail { get; set; }

        public string fromEmailPassword { get; set; }
    }
}
