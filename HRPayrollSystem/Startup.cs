using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HRPayrollSystem
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDistributedMemoryCache();
            services.AddDbContext<PayroleDbContext>(pdc =>
            {
                pdc.UseSqlServer(Configuration["Database:PayroleDb"]);
            });
            services.AddSession();
            services.AddIdentity<IdentityUser, IdentityRole>()
                                                          .AddDefaultTokenProviders()
                                                            .AddEntityFrameworkStores<PayroleDbContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Hr}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                   template: "{controller}/{action}/{id?}",
                   defaults: new { controller = "Account", action = "Login" });
            });
        }
    }
}
