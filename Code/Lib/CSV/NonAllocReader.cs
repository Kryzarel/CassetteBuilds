using System;
using System.IO;
using System.Runtime.CompilerServices;
using Kryz.Collections;

namespace Kryz.CSV
{
	public ref struct NonAllocReader
	{
		private const string lineEndingsStr = "\r\n";

		private ReadOnlySpan<char> current;
		private ReadOnlySpan<char> toRead;
		private NonAllocBuffer<char> buffer;
		private readonly TextReader? reader;
		private readonly ReadOnlySpan<char> lineEndings;

		public readonly ReadOnlySpan<char> Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => current;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NonAllocReader(ReadOnlySpan<char> text)
		{
			current = default;
			toRead = text;
			buffer = default;
			reader = null;
			lineEndings = lineEndingsStr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public NonAllocReader(TextReader reader, in Span<char> buffer = default)
		{
			current = default;
			toRead = default;
			this.buffer = buffer;
			this.reader = reader;
			lineEndings = lineEndingsStr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<char> ReadLine(bool skipEmpty)
		{
			TryReadLine(skipEmpty, out ReadOnlySpan<char> line);
			return line;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadLine(bool skipEmpty)
		{
			return TryReadLine(skipEmpty, out _);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryReadLine(bool skipEmpty, out ReadOnlySpan<char> line)
		{
			if (Peek(skipEmpty, out ReadOnlySpan<char> tmpLine))
			{
				AdvanceLine(tmpLine.Length);
				current = tmpLine;
				line = tmpLine;
				return true;
			}
			current = toRead;
			toRead = default;
			line = tmpLine;
			return !current.IsEmpty;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Peek(bool skipEmpty, out ReadOnlySpan<char> line)
		{
			bool success;
			while ((success = Peek(out line)) && line.IsEmpty && skipEmpty)
			{
				toRead = toRead.TrimStart(lineEndings);
			}
			return success;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Peek(out ReadOnlySpan<char> line)
		{
			do
			{
				int index = toRead.IndexOfAny(lineEndings);
				if (index >= 0)
				{
					line = toRead[..index];
					return true;
				}
			}
			while (TryRead());
			line = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AdvanceLine(int length)
		{
			// Treat '\r\n' as a single line ending, since that's the default on Windows
			int offset = toRead[length..].StartsWith(lineEndings) ? 2 : 1;
			toRead = toRead[(length + offset)..];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool TryRead()
		{
			if (reader == null) return false;

			// Copy what's left to read to the start of the buffer
			if (toRead.Length > 0)
			{
				toRead.CopyTo(buffer);
				toRead = buffer.Span[..toRead.Length];
				buffer.Count = toRead.Length;
			}
			// Increase buffer size if needed
			if (toRead.Length >= buffer.Capacity)
			{
				buffer.EnsureCapacity(buffer.Capacity * 2);
			}
			// Read into buffer appending data after what's still left to read
			buffer.Count = reader.Read(buffer.Span[toRead.Length..]);
			toRead = buffer.Span[..(toRead.Length + buffer.Count)];
			return buffer.Count > 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Dispose()
		{
			buffer.Dispose();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator NonAllocReader(ReadOnlySpan<char> span) => new(span);
	}
}