using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiSecuritySampleProject
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
            services.AddControllers();

            // JWT Token Auth with IdentityServer4
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(o =>
                {
                    o.Authority = Configuration["IdentityServerSettings:Uri"];
                    o.ApiName = Configuration["IdentityServerSettings:Scope"];
                    o.RequireHttpsMetadata = true;
                    o.EnableCaching = true;
                    o.CacheDuration = TimeSpan.FromMinutes(30);
                });


            // Example Scope Policies with a simple read or write
            services.AddAuthorization(o =>
            {
                o.AddPolicy("Writer", p =>
                {
                    p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    p.RequireAuthenticatedUser();
                    p.RequireScope("name-of-api.write");
                });
                o.AddPolicy("Reader", p =>
                {
                    p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    p.RequireAuthenticatedUser();
                    p.RequireScope("name-of-api.read");
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
