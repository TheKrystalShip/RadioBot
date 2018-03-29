namespace RadioBot.Database.Models
{
	public class Radio
	{
		public string Name { get; set; }
		public string Url { get; set; }
		
		public virtual User User { get; set; }

		public Radio(string name, string url)
		{
			Name = name;
			Url = url;
		}
	}
}