using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ChamadosTI.Data;

public class ContextoChamadosFactory : IDesignTimeDbContextFactory<ContextoChamados>
{
    public ContextoChamados CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<ContextoChamados>();
        var versaoMySql = ServerVersion.Parse("8.0.0");
        optionsBuilder.UseMySql(connectionString, versaoMySql);

        return new ContextoChamados(optionsBuilder.Options);
    }
}
