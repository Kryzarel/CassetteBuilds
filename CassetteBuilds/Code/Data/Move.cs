namespace CassetteBuilds.Code.Data
{
	public readonly struct Move
	{
		public readonly int Index;
		public readonly string Name;
		public readonly string Type;
		public readonly string Category;
		public readonly int Power;
		public readonly int Accuracy;
		public readonly int Cost;
		public readonly string WikiLink;

		public Move(int index, string name, string type, string category, int power, int accuracy, int cost, string wikiLink)
		{
			Index = index;
			Name = name;
			Type = type;
			Category = category;
			Power = power;
			Accuracy = accuracy;
			Cost = cost;
			WikiLink = wikiLink;
		}

		public override readonly string ToString()
		{
			return $"{Index} | {Name} | {Type} | {Category} | {Power} | {Accuracy} | {Cost} | {WikiLink}";
		}
	}
}