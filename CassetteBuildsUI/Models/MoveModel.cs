using System;
using System.IO;
using CassetteBuilds.Code.Data;

namespace CassetteBuildsUI.Models
{
	public class MoveModel
	{
		public int Index { get; }
		public string Name { get; }
		public string Type { get; }
		public string Category { get; }
		public int Power { get; }
		public int Accuracy { get; }
		public int Cost { get; }
		public string WikiLink { get; }
		public string TypeImagePath { get; }

		public MoveModel(Move move)
		{
			Index = move.Index;
			Name = move.Name;
			Type = move.Type;
			Category = move.Category;
			Power = move.Power;
			Accuracy = move.Accuracy;
			Cost = move.Cost;
			WikiLink = move.WikiLink;
			TypeImagePath = Path.ChangeExtension(Path.Combine(AppContext.BaseDirectory, "Images", Type), ".png");
		}
	}
}