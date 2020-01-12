using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HRPayrollSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webhost = CreateWebHostBuilder(args).Build();
            using (IServiceScope scope = webhost.Services.CreateScope())
            {
                using (PayroleDbContext db = scope.ServiceProvider.GetRequiredService<PayroleDbContext>())
                {
                    Seed.InvokeAsync(scope, db).Wait();
                }
            }
            webhost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
