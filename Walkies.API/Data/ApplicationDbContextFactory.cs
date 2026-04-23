using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Walkies.API.Data
{
    /// <summary>
    /// Design time factory for ApplicationDbContext. Used by EF Core tooling (migrations)
    /// to create an instance of the DbContext at design time without needing to run the full application.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// Creates a new instance of ApplicationDbContext for use by EF Core design time tools.
        /// </summary>
        /// <param name="args">Arguments passed by EF Core tooling</param>
        /// <returns>A configured instance of ApplicationDbContext</returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}