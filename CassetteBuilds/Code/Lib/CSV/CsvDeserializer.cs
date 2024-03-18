using System;
using System.IO;
using Kryz.Collections;

namespace Kryz.CSV
{
	public static class CsvDeserializer
	{
		public delegate T CreateObject<T>(in CsvRow row);

		public enum ColumnType { Both, CSV, TSV }

		public static T[] FromString<T>(string csv, CreateObject<T> createObject)
		{
			return Deserialize(csv, textReader: null, columnType: ColumnType.Both, createObject: createObject);
		}

		public static T[] FromFile<T>(string path, CreateObject<T> createObject)
		{
			using StreamReader streamReader = File.OpenText(path);
			ColumnType columnType = ColumnType.Both;
			if (path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
				columnType = ColumnType.CSV;
			else if (path.EndsWith(".tsv", StringComparison.OrdinalIgnoreCase))
				columnType = ColumnType.TSV;
			return Deserialize(path, streamReader, columnType, createObject);
		}

		public static T[] FromTextReader<T>(TextReader textReader, CreateObject<T> createObject, ColumnType columnType = ColumnType.Both)
		{
			return Deserialize(null, textReader, columnType, createObject);
		}

		private static T[] Deserialize<T>(string? str, TextReader? textReader, ColumnType columnType, CreateObject<T> createObject)
		{
			using NonAllocReader reader = textReader != null ? new(textReader, stackalloc char[1024]) : new(str);

			if (!reader.Peek(out _)) return Array.Empty<T>();

			ReadOnlySpan<char> columnSeparators = columnType switch
			{
				ColumnType.CSV => stackalloc char[] { ',' },
				ColumnType.TSV => stackalloc char[] { '\t' },
				_ => stackalloc char[] { ',', '\t' },
			};

			// Read first line
			ReadOnlySpan<char> headerText = reader.ReadLine(skipEmpty: true);
			int numSeparators = headerText.CountSeparators(columnSeparators);
			using NonAllocBuffer<int> separatorCache = numSeparators <= 128 ? new(stackalloc int[numSeparators], numSeparators) : new(numSeparators, numSeparators);

			using NonAllocBuffer<T> objectBuffer = new(10);
			for (int i = 0; reader.TryReadLine(skipEmpty: true); i++)
			{
				CsvRow row = new(i, reader.Current, columnSeparators, separatorCache);
				objectBuffer.Add(createObject(row));
			}
			return objectBuffer.ToArray();
		}
	}
}