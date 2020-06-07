using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using TaskScheduler.Web.Configs;
using TaskScheduler.Web.Extensions;
using TaskScheduler.Web.Infrastructure;
using TaskScheduler.Web.Infrastructure.Filters;
using TaskScheduler.Web.Infrastructure.Jobs;
using TaskScheduler.Web.Infrastructure.Repository;
using DateTimeConverter = TaskScheduler.Web.Extensions.DateTimeConverter;

namespace TaskScheduler.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigHelper.SetInstance(configuration);
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,options => {
                    options.Cookie.HttpOnly = true;
                    options.LoginPath = new PathString("/home/login");
                    options.LogoutPath = new PathString("/home/signout");
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                });
            services.Configure<AppSettings>(Configuration);
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews(options=> {
                options.Filters.Add(typeof(TaskAuthorizeFilter));
            }).AddJsonOptions(options=> {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseTaskScheduler(env);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=List}/{id?}");
            }); 
        }
    }
}
