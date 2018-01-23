using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Autofac;
using Autofac.Integration.WebApi;
using System.Web.Mvc;
using System.Web.Http;
using Autofac.Integration.Mvc;
using System.Reflection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

[assembly: OwinStartup(typeof(MissionskyOA.Api.Startup))]

namespace MissionskyOA.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }

        
    }
}
