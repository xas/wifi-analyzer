using Microsoft.EntityFrameworkCore;
using WiFiAnalyzer.Core.Models;

namespace WiFiAnalyzer.Core.Database;

public class WiFiAnalyzerContext : DbContext
{
    public DbSet<WiFiNetwork> WiFiNetworks { get; set; } = null!;

    public WiFiAnalyzerContext(DbContextOptions<WiFiAnalyzerContext> options) : base(options)
        => Database.EnsureCreated();
}
