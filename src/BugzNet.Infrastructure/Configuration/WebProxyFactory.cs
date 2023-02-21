using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.Configuration
{
    public static class WebProxyFactory
    {
        public static WebProxy Create(ProxySettings settings)
        {
            var address = new Uri(settings.Host);

            NetworkCredential credentials = null;
            if (!string.IsNullOrEmpty(settings.User) && !string.IsNullOrEmpty(settings.Password))
                credentials = new NetworkCredential(settings.User, settings.Password);

            return new WebProxy()
            {
                Address = address,
                Credentials = credentials,
                BypassList = settings.BypassList
            };
        }
    }
}
