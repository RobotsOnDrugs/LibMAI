﻿namespace LibMai.Cards.Util;

public static class Helpers
{
	public static bool TryFindSequence(Stream stream, in ImmutableArray<byte> sequence, out long pos)
	{
		long _originalPos = stream.Position;
		long _bufferPos = stream.Position;
		long _currentPos;
		const int bufSize = 4096;
		byte[] buffer = new byte[bufSize];
		byte[] potentialMatch = new byte[sequence.Length];
		long _potentialMatchPos;

		int readBytes = stream.Read(buffer, 0, bufSize);
		while (readBytes > 0)
		{
			_currentPos = stream.Position;
			for (int i = 0; i < readBytes; i++)
			{
				if (buffer[i] != sequence[0]) continue;
				_potentialMatchPos = _bufferPos + i;
				// if ((i + sequence.Length) > (bufSize - 1)) // in case the sequence goes beyond the buffer
				_ = stream.Seek(_potentialMatchPos, SeekOrigin.Begin);
				_ = stream.Read(potentialMatch, 0, sequence.Length);
				if (potentialMatch.SequenceEqual(sequence))
				{
					_ = stream.Seek(_originalPos, SeekOrigin.Begin);
					pos = _potentialMatchPos;
					return true;
				}
				_ = stream.Seek(_currentPos, SeekOrigin.Begin);

				// Some bad code that tries to iterate over the buffer bytes instead of pulling from the stream again.
				// The other way is fast enough, so I won't bother to fix this unless speed becomes an issue.
				//bool _sequenceMatch = true;
				//for (int j = 1; j < sequence.Length; j++)
				//{
				//	if (buffer[i + j] != sequence[j]) { _sequenceMatch = false; break; }
				//	Console.WriteLine($"{buffer[i + j]} = {sequence[j]}");
				//}
				//stream.Seek(_originalPos, SeekOrigin.Begin);
				//if (_sequenceMatch) { return _potentialMatchPos; }
			}

			readBytes = stream.Read(buffer, 0, bufSize);
			_bufferPos += bufSize;
		}
		pos = _originalPos;
		_ = stream.Seek(_originalPos, SeekOrigin.Begin);
		return false;
	}

	public static float[] GetRepeatArray(in int length, in float val)
	{
		float[] _a = new float[length];
		for (int i = 0; i < length; i++) _a[i] = val;
		return _a;
	}
	public static byte[][] GetDataChunks(in byte[] bytes, in int numChunks)
	{
		byte[][] chunks = new byte[numChunks][];
		using MemoryStream _mstream = new(bytes);
		using BinaryReader _reader = new(_mstream);
		for (int i = 0; i < chunks.Length; i++)
		{
			int _count = _reader.ReadInt32();
			byte[] _bytes = _reader.ReadBytes(_count);
			chunks[i] = _bytes;
		}
		return chunks;
	}

	public static ImmutableArray<ImmutableArray<Vector3>> GetImmutable2DVector3(Vector3[,] twodvector3)
	{
		ImmutableArray<ImmutableArray<Vector3>>.Builder _outerArrayBuilder = ImmutableArray.CreateBuilder<ImmutableArray<Vector3>>();
		if (twodvector3.Length == 0)
			_outerArrayBuilder.Add(ImmutableArray<Vector3>.Empty);
		for (int i = 0; i < twodvector3.GetLength(0); i++)
		{
			ImmutableArray<Vector3>.Builder _innerArrayBuilder = ImmutableArray.CreateBuilder<Vector3>();
			for (int j = 0; j < twodvector3.GetLength(i); j++)
			{
				_innerArrayBuilder.Add(twodvector3[i, j]);
			}
			_outerArrayBuilder.Add(_innerArrayBuilder.ToImmutableArray());
		}
		return _outerArrayBuilder.ToImmutableArray();
	}
	public static ImmutableArray<float> UnpackFloats(ref MessagePackReader reader, in int expectedLength)
	{
		if (reader.TryReadNil())
			throw new InvalidOperationException("typecode is null, struct not supported");
		int count = reader.ReadArrayHeader();
		if (count != expectedLength) throw new InvalidOperationException($"Got a float array with an unexpected length. Expected {expectedLength}, got {count}");
		float[] _a = new float[expectedLength];
		for (int i = 0; i < count; i++)
			_a[i] = reader.ReadSingle();
		return _a.ToImmutableArray();
	}
}
