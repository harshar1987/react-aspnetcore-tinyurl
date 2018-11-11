using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using UrlShortnerApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using UrlShortnerApi.DAL;
using UrlShortnerApi.Services;
using UrlShortnerApi.Dependencies;

namespace UrlShortnerApi
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
            services.AddScoped<IShortUrlService, ShortUrlService>();
            services.AddScoped<ISystemClock, SystemClock>();
            services.AddSingleton<IDocumentDBRepository<ShortUrl>>(new DocumentDBRepository<ShortUrl>("UrlShortner", "ShortUrls"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new Info
                {
                    Title = "Url Shortner Api",
                    Description = "An Asp.net Core API for tiny Url",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            app.UseSwagger();
            app.UseSwaggerUI(sw =>
            {
                sw.SwaggerEndpoint("/swagger/v1/swagger.json", "Url Shortner Api v1");
                sw.RoutePrefix = string.Empty;
            });
            //app.UseMvc();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "shorturl",
                    template: "{*hash}",
                    defaults: new { controller = "shorturl", Action="GetOriginalUrlAsync"});
            });
        }
    }
}
