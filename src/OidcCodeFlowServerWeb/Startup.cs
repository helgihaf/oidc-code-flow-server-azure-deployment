using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Landsbanki.Authentication.OidcCodeFlowServer;

namespace Landsbanki.Authentication.OidcCodeFlowServerWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AuthSettings>(Configuration.GetSection("Auth"));
            services.AddCodeCallback();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            var authSettings = app.ApplicationServices.GetService<IOptions<AuthSettings>>().Value;
            app.UseCors(builder =>
            {
                builder.WithMethods("GET", "POST");
                if (authSettings.AllowedCORSHeaders == "*")
                    builder.AllowAnyHeader();
                else builder.WithHeaders(authSettings.AllowedCORSHeaders.Split(','));
                if (authSettings.AllowedCORSOrigins == "*")
                    builder.AllowAnyOrigin();
                else builder.WithOrigins(authSettings.AllowedCORSOrigins.Split(','));
            });
            app.UseHttpsRedirection();
            var callbackEndpoint = (authSettings.RelativeCallbackEndpoint.StartsWith('/')) ? authSettings.RelativeCallbackEndpoint : "/" + authSettings.RelativeCallbackEndpoint;
            var refreshEndpoint = (authSettings.RelativeRefreshEndpoint.StartsWith('/')) ? authSettings.RelativeRefreshEndpoint : "/" + authSettings.RelativeRefreshEndpoint;
            app.UseOidcCodeCallback(callbackEndpoint);
            app.UseRefresh(refreshEndpoint);
            app.UseMvc();
        }
    }
}
