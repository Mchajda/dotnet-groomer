using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration["Values:MySqlConnectionString"];
        Console.WriteLine(connectionString);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("The connection string 'MySqlConnectionString' was not found.");
        }

        var builder = new DbContextOptionsBuilder<MyDbContext>();
        builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new MyDbContext(builder.Options);
    }
}
