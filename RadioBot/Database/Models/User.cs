using System.Collections.Generic;

namespace RadioBot.Database.Models
{
	public class User
    {
		public string Name { get; set; }
		public string Discriminator { get; set; }

		public virtual List<Radio> Radios { get; set; }

		public User(string name, string discriminator)
		{
			Name = name;
			Discriminator = discriminator;
		}
    }
}
