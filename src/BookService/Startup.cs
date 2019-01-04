using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer, TelemetryProperties>();
            var instrumentationKey = Environment.GetEnvironmentVariable("INSTRUMENTATION_KEY");
            services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { DeveloperMode = true , InstrumentationKey = instrumentationKey});
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }

    internal class TelemetryProperties : ITelemetryInitializer
    {
        private IConfiguration configuration;

        public TelemetryProperties(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var versionTag = Environment.GetEnvironmentVariable("VERSIONTAG");
            if (!telemetry.Context.GlobalProperties.Keys.Contains("VersionTag"))
            {
                telemetry.Context.GlobalProperties.Add("VersionTag", versionTag);
            }
        }

    }

}
