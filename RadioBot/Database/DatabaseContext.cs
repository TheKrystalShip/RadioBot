using Microsoft.EntityFrameworkCore;

using RadioBot.Database.Models;
using RadioBot.Properties;

namespace RadioBot.Database
{
	public class DatabaseContext : DbContext
    {
		public DbSet<Radio> Radios { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder ob)
		{
			ob.UseSqlServer(Resources.ConnectionString);
		}

		protected override void OnModelCreating(ModelBuilder mb)
		{
			mb.Entity<User>()
				.HasMany(x => x.Radios)
				.WithOne(x => x.User);

			mb.Entity<Radio>()
				.HasOne(x => x.User)
				.WithMany(x => x.Radios)
				.OnDelete(DeleteBehavior.SetNull);
		}
    }
}
