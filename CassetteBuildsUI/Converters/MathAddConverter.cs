using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CassetteBuildsUI.Converters
{
	public class MathAddConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			// For add this is simple. just return the sum of the value and the parameter.
			if (value is double v1 && parameter is double v2)
			{
				return v1 + v2;
			}
			return null;
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			// If we want to convert back, we need to subtract instead of add.
			if (value is double v1 && parameter is double v2)
			{
				return v1 - v2;
			}
			return null;
		}
	}
}