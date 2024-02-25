using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using dotnet_groomer.Repositories;
using dotnet_groomer.Repositories.Interfaces;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IProductRepository, ProductSqlRepository>();

            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseMySql(Environment.GetEnvironmentVariable("MySqlConnectionString"),
                ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("MySqlConnectionString"))));
        }
    }
}
