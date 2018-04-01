using Microsoft.EntityFrameworkCore;

using RadioBot.Database.Models;
using RadioBot.Properties;

namespace RadioBot.Database
{
	public class DatabaseContext : DbContext
    {
		private static string ConnectionString = Resources.ConnectionString;
		public DbSet<Radio> Radios { get; set; }

		public DatabaseContext() : base()
		{

		}

		public DatabaseContext(DbContextOptions options) : base(options)
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseSqlServer(ConnectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>()
				.HasMany(x => x.Radios)
				.WithOne(x => x.User);

			modelBuilder.Entity<Radio>()
				.HasOne(x => x.User)
				.WithMany(x => x.Radios)
				.OnDelete(DeleteBehavior.SetNull);
		}
    }
}
