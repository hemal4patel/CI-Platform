using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Entities.Auth
{
    public class JwtSetting
    {
        public string Key { get; set; } = null!;

        public string Issuer { get; set; } = null!;

        public string Audience { get; set; } = null!;
    }
}
