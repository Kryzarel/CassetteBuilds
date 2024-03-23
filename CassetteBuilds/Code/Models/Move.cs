using Avalonia.Media.Imaging;
using CassetteBuilds.Code.Logic;

namespace CassetteBuilds.Code.Models
{
	public class Move
	{
		public int Index { get; }
		public string Name { get; }
		public string Type { get; }
		public string Category { get; }
		public int Power { get; }
		public int Accuracy { get; }
		public int Cost { get; }
		public string WikiLink { get; }

		public Bitmap TypeImage { get; }

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

			TypeImage = TypeImageDatabase.GetImage(Type);
		}

		public override string ToString()
		{
			return Name;
		}

		public string ToString_Full()
		{
			return $"{Index} | {Name} | {Type} | {Category} | {Power} | {Accuracy} | {Cost} | {WikiLink}";
		}
	}
}