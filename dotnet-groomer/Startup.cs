using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseMySql(Environment.GetEnvironmentVariable("MySqlConnectionString"),
                ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("MySqlConnectionString"))));
        }
    }
}
