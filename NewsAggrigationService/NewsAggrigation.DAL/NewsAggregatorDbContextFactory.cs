using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NewsAggrigation.DAL
{
    internal class NewsAggregatorDbContextFactory : IDesignTimeDbContextFactory<NewsAggregatorDbContext>
    {
        public NewsAggregatorDbContext CreateDbContext(string[] args)
        {
            // Build configuration by locating appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../NewsAggrigationService"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("NewsAggregatorDB");

            var optionsBuilder = new DbContextOptionsBuilder<NewsAggregatorDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new NewsAggregatorDbContext(optionsBuilder.Options);
        }
    }
}
