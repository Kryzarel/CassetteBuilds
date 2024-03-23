namespace CassetteBuilds.Code.Logic
{
	public static class Features
	{
		private static IUrlOpener? urlOpener;
		public static IUrlOpener UrlOpener { get => urlOpener ??= new DefaultUrlOpener(); set => urlOpener = value; }
	}
}