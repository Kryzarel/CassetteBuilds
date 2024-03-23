using System.Threading.Tasks;
using CassetteBuilds.Code.Admin;

namespace CassetteBuilds.Admin
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await DataUpdater.UpdateAll();
		}
	}
}