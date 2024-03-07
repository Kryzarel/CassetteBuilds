using System;
using System.IO;
using Kryz.Collections;

namespace Kryz.CSV
{
	public static class CsvDeserializer
	{
		public delegate T CreateObject<T>(int index, in ReadOnlySpan<char> row, in ReadOnlySpan<int> separatorCache);

		public static T[] Deserialize<T>(string path, CreateObject<T> createObject)
		{
			using StreamReader streamReader = File.OpenText(path);
			using NonAllocReader reader = new(streamReader, stackalloc char[1024]);

			ReadOnlySpan<char> columnSeparators = stackalloc char[] { ',' };

			// Read first line
			ReadOnlySpan<char> headerText = reader.ReadLine(skipEmpty: true);
			int numSeparators = headerText.CountSeparators(columnSeparators);
			using NonAllocBuffer<int> separatorCache = numSeparators <= 128 ? new(stackalloc int[numSeparators], numSeparators) : new(numSeparators, numSeparators);
			headerText.FindSeparators(columnSeparators, separatorCache);

			using NonAllocBuffer<T> objectBuffer = new(10);
			for (int i = 0; reader.TryReadLine(skipEmpty: true); i++)
			{
				ReadOnlySpan<char> row = reader.Current;
				row.FindSeparators(columnSeparators, separatorCache);
				objectBuffer.Add(createObject(i, row, separatorCache));
			}
			return objectBuffer.ToArray();
		}
	}
}