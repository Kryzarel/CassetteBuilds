namespace Cassette_Builds.Code.Admin
{
	public struct Move
	{
		public string Name;
		public string Type;
		public string Category;
		public int Power;
		public int Accuracy;
		public int Cost;
		public string WikiLink;

		public override readonly string ToString()
		{
			return $"{Name} | {Type} | {Category} | {Power} | {Accuracy} | {Cost} | {WikiLink}";
		}
	}
}