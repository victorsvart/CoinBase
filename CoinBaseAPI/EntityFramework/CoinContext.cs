using CoinBase.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace EntityFramework
{

    public class CoinContext : DbContext
    {
        protected readonly IConfiguration Configuration;

     
        // Set SqlServer 
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("WebApiDatabase");
                options.UseSqlServer(connectionString);
            }
        }

        public DbSet<Coin> Coins { get; set; }
        public DbSet<CoinConverter> CoinConverters { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coin>().ToTable("Coin");
            modelBuilder.Entity<CoinConverter>().ToTable("Converters");

            // configures one-to-many relationship
            modelBuilder.Entity<Coin>()
                .HasMany(x => x.Converters)
                .WithOne(x => x.Coin)
                .HasForeignKey(x => x.IDCoin)
                .HasPrincipalKey(x => x.ID);
        }







    }
}





