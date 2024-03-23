using System;

namespace CassetteBuilds.Code.Admin
{
	/// <summary>
	/// Parses and individual type html page
	/// </summary>
	public static class TypeHtmlParser
	{
		public static string Parse(in ReadOnlySpan<char> html, in ReadOnlySpan<char> baseUrl)
		{
			const StringComparison cmp = StringComparison.OrdinalIgnoreCase;
			ReadOnlySpan<char> imageClass = html[html.IndexOf("class=\"infobox-image\"", cmp)..];
			ReadOnlySpan<char> imageLink = imageClass.GetBetween("src=\"", "\"");
			return baseUrl.ConcatToString(imageLink);
		}
	}
}