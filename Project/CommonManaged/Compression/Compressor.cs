// Copyright 2019. All Rights Reserved.
namespace GameFramework.Common.Compression
{
	public static class Compressor
	{
		private const uint HLOG = 14;
		private const uint HSIZE = (1 << 14);
		private const uint MAX_LIT = (1 << 5);
		private const uint MAX_OFF = (1 << 13);
		private const uint MAX_REF = ((1 << 8) + (1 << 3));

		public static byte[] Compress(byte[] Input)
		{
			return Compress(Input, 0, (uint)Input.Length);
		}

		public static byte[] Compress(byte[] Input, uint Index, uint Length)
		{
			byte[] processed = new byte[Length];
			int length = Compress(Input, Index, Length, processed, 0, (uint)processed.Length);

			byte[] output = new byte[length];
			System.Array.Copy(processed, 0, output, 0, length);

			return output;
		}

		public static int Compress(byte[] Input, uint InputIndex, uint InputLength, byte[] Output, uint OutputIndex, uint OutputLength)
		{
			long[] hashTable = new long[HSIZE];

			long hslot;
			uint iidx = InputIndex;
			uint oidx = OutputIndex;
			long reference;

			uint hval = (uint)(((Input[iidx]) << 8) | Input[iidx + 1]); // FRST(in_data, iidx);
			long off;
			int lit = 0;

			for (; ; )
			{
				if (iidx < InputLength - 2)
				{
					hval = (hval << 8) | Input[iidx + 2];
					hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1));
					reference = hashTable[hslot];
					hashTable[hslot] = (long)iidx;


					if ((off = iidx - reference - 1) < MAX_OFF
						&& iidx + 4 < InputLength
						&& reference > 0
						&& Input[reference + 0] == Input[iidx + 0]
						&& Input[reference + 1] == Input[iidx + 1]
						&& Input[reference + 2] == Input[iidx + 2]
						)
					{
						/* match found at *reference++ */
						uint len = 2;
						uint maxlen = (uint)InputLength - iidx - len;
						maxlen = maxlen > MAX_REF ? MAX_REF : maxlen;

						if (oidx + lit + 1 + 3 >= OutputLength)
							return 0;

						do
							len++;
						while (len < maxlen && Input[reference + len] == Input[iidx + len]);

						if (lit != 0)
						{
							Output[oidx++] = (byte)(lit - 1);
							lit = -lit;
							do
								Output[oidx++] = Input[iidx + lit];
							while ((++lit) != 0);
						}

						len -= 2;
						iidx++;

						if (len < 7)
						{
							Output[oidx++] = (byte)((off >> 8) + (len << 5));
						}
						else
						{
							Output[oidx++] = (byte)((off >> 8) + (7 << 5));
							Output[oidx++] = (byte)(len - 7);
						}

						Output[oidx++] = (byte)off;

						iidx += len - 1;
						hval = (uint)(((Input[iidx]) << 8) | Input[iidx + 1]);

						hval = (hval << 8) | Input[iidx + 2];
						hashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
						iidx++;

						hval = (hval << 8) | Input[iidx + 2];
						hashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
						iidx++;
						continue;
					}
				}
				else if (iidx == InputLength)
					break;

				/* one more literal byte we must copy */
				lit++;
				iidx++;

				if (lit == MAX_LIT)
				{
					if (oidx + 1 + MAX_LIT >= OutputLength)
						return 0;

					Output[oidx++] = (byte)(MAX_LIT - 1);
					lit = -lit;
					do
						Output[oidx++] = Input[iidx + lit];
					while ((++lit) != 0);
				}
			}

			if (lit != 0)
			{
				if (oidx + lit + 1 >= OutputLength)
					return 0;

				Output[oidx++] = (byte)(lit - 1);
				lit = -lit;
				do
					Output[oidx++] = Input[iidx + lit];
				while ((++lit) != 0);
			}

			return (int)oidx;
		}

		public static byte[] Decompress(byte[] Input)
		{
			return Decompress(Input, 0, (uint)Input.Length, 1000);
		}

		public static byte[] Decompress(byte[] Input, uint Index, uint Length, uint SizeMultiplier)
		{
			byte[] processed = new byte[Length * SizeMultiplier];
			int length = Decompress(Input, Index, Length, processed, 0, (uint)processed.Length);

			byte[] output = new byte[length];
			System.Array.Copy(processed, 0, output, 0, length);

			return output;
		}

		public static int Decompress(byte[] Input, uint InputIndex, uint InputLength, byte[] Output, uint OutputIndex, uint OutputLength)
		{
			uint iidx = 0;
			uint oidx = 0;

			do
			{
				uint ctrl = Input[iidx++];

				if (ctrl < (1 << 5)) /* literal run */
				{
					ctrl++;

					if (oidx + ctrl > OutputLength)
					{
						//SET_ERRNO (E2BIG);
						return 0;
					}

					do
						Output[oidx++] = Input[iidx++];
					while ((--ctrl) != 0);
				}
				else /* back reference */
				{
					uint len = ctrl >> 5;

					int reference = (int)(oidx - ((ctrl & 0x1f) << 8) - 1);

					if (len == 7)
						len += Input[iidx++];

					reference -= Input[iidx++];

					if (oidx + len + 2 > OutputLength)
					{
						//SET_ERRNO (E2BIG);
						return 0;
					}

					if (reference < 0)
					{
						//SET_ERRNO (EINVAL);
						return 0;
					}

					Output[oidx++] = Output[reference++];
					Output[oidx++] = Output[reference++];

					do
						Output[oidx++] = Output[reference++];
					while ((--len) != 0);
				}
			}
			while (iidx < InputLength);

			return (int)oidx;
		}
	}
}
