namespace Cassette_Builds
{
	public readonly struct Move
	{
		public readonly string Name;
		public readonly string Type;
		public readonly string Category;
		public readonly int Power;
		public readonly int Accuracy;
		public readonly int Cost;
		public readonly string Link;

		public Move(string name, string type, string category, int power, int accuracy, int cost, string link)
		{
			Name = name;
			Type = type;
			Category = category;
			Power = power;
			Accuracy = accuracy;
			Cost = cost;
			Link = link;
		}
	}
}