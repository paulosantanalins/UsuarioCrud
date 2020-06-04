using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UsuarioApi.Infra.Data.SqlServer.Mappings;
using System.IO;
using Utils;

namespace UsuarioApi.Infra.Data.SqlServer.Context
{
    public class AppDbContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioMap());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Variables.EnvironmentName}.json", optional: true)
                .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
    }
}
