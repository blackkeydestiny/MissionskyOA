using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace MissionskyOA.Web.Handlers
{
    public class SecurityHandler : DelegatingHandler
    {
        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {       
            return base.SendAsync(request, cancellationToken);
        }


        private static string GetHeaderByKey(HttpRequestMessage request, string key)
        {
            string value = null;
            try
            {
                value = request.Headers.GetValues(key).First();
            }
            catch
            {
            }
            return value;
        }
    }
}
