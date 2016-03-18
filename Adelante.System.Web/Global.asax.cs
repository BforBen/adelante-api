using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

using Serilog;
using SerilogWeb.Classic.Enrichers;

namespace Adelante.System.Web
{
    public class WebApiApplication : global::System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.LowercaseUrls = true;

            //var Logger = new LoggerConfiguration()
            //    .WriteTo.File(HttpContext.Current.Server.MapPath("~/App_Data/log.txt"))
            //    .Enrich.WithMachineName()
            //    .Enrich.With<HttpRequestUrlEnricher>()
            //    .Enrich.WithThreadId()
            //    .Enrich.With<HttpRequestClientHostIPEnricher>()
            //    .Enrich.WithProcessId()
            //    .Enrich.With<HttpRequestIdEnricher>()
            //    .Enrich.With<HttpRequestTypeEnricher>()
            //    .Enrich.With<HttpRequestUserAgentEnricher>()
            //    .Enrich.With<HttpRequestUrlReferrerEnricher>();

            //Log.Logger = Logger.CreateLogger();

            //Log.Debug("Payments API started.");
        }
    }
}