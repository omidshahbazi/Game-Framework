// Copyright 2019. All Rights Reserved.
using System;
using System.Security.Cryptography;

namespace GameFramework.Common.Utilities
{
	public class Random
	{
		private CSPRNG randomGenerator = null;
		
		public int Seed
		{
			get;
			private set;
		}

		public Random()
		{
			randomGenerator = new CSPRNG();
		}

		public Random(int Seed)
		{
			this.Seed = Seed;

			byte[] seed = new byte[48];

			int index = 0;
			for (int i = 0; i < 12; ++i)
			{
				byte[] bytes = BitConverter.GetBytes(Seed);
				seed[index++] = bytes[0];
				seed[index++] = bytes[1];
				seed[index++] = bytes[2];
				seed[index++] = bytes[3];
			}

			randomGenerator = new CSPRNG(seed);
		}

		public int Next(int Min, int Max)
		{
			return randomGenerator.random(Min, Max - 1);
		}

		public float Next(float Min, float Max)
		{
			float precision = 1000.0F;
			return Next((int)(Min * precision), (int)(Max * precision)) / precision;
		}
	}

	class CSPRNG
	{
		private UInt32[] _key;
		private UInt32[] _nonce;
		private UInt32 _block_number;

		private UInt32[] _curr_gen_block;
		private int _curr_used;

		public static byte[] MakeRandomSeed()
		{
			byte[] ret = new byte[48];

			RandomNumberGenerator rng = new RNGCryptoServiceProvider();
			rng.GetBytes(ret);

			return ret;
		}

		public CSPRNG() :
			this(MakeRandomSeed())
		{
		}

		public CSPRNG(byte[] seed)    // seed MUST be 48 bytes.
		{
			_key = ChachaCipher.ExtractKey(seed);
			_nonce = ChachaCipher.ExtractNonce(seed);
			_block_number = ChachaCipher.Slice(seed, 44);

			_curr_used = 16;
		}

		public UInt32 random()
		{
			if (_curr_used >= 16)
				refill();
			return _curr_gen_block[_curr_used++];
		}

		public int random(UInt32 upper_bound) // In range [0..upper_bound), i.e. upper_bound is EXCLUDED
		{
			UInt32 limit = 0xFFFFFFFF - (0xFFFFFFFF % upper_bound); // I should have used 2^32 instead of 0xFFFFFFFF, but I think this is OK too.
			UInt32 r = random();
			while (r >= limit)
				r = random();
			return (int)(r % upper_bound);
		}

		public int random(int lo_inclusive, int hi_inclusive) // Both are inclusive
		{
			if (lo_inclusive <= hi_inclusive)
				return random((UInt32)(hi_inclusive - lo_inclusive + 1)) + lo_inclusive;
			else
				return random((UInt32)(lo_inclusive - hi_inclusive + 1)) + hi_inclusive;
		}

		private void refill()
		{
			_curr_gen_block = ChachaCipher.GenStream(_key, _block_number++, _nonce);
			_curr_used = 0;
		}
	}

	class ChachaCipher
	//<RoundPairs>
	//where RoundPairs: uint
	{
		const int RoundPairs = 4;

		// key is 8x32 bits, nonce is 3x32 bits.
		public static UInt32[] GenStream(UInt32[] key, UInt32 block_number, UInt32[] nonce)
		{
			var state = new UInt32[16] {
				0x61707865, 0x3320646e, 0x79622d32, 0x6b206574,
				key[0], key[1], key[2], key[3],
				key[4], key[5], key[6], key[7],
				block_number, nonce[0], nonce[1], nonce[2]
			};

			var working_state = state.Clone() as UInt32[];

			for (int i = 0; i < RoundPairs; ++i)
				roundpair(working_state);

			mix(state, working_state);

			return state;
		}

		// key_and_nonce must be exactly 352 bits, or 44 bytes.
		public static byte[] GenStream(byte[] key_and_nonce, UInt32 block_number)
		{
			return SerializeKeyStream(
				GenStream(ExtractKey(key_and_nonce), block_number, ExtractNonce(key_and_nonce))
			);
		}

		public static UInt32[] ExtractKey(byte[] combined_key_and_nonce)
		{
			return new UInt32[] {
				Slice(combined_key_and_nonce, 0), Slice(combined_key_and_nonce, 4), Slice(combined_key_and_nonce, 8), Slice(combined_key_and_nonce,12),
				Slice(combined_key_and_nonce,16), Slice(combined_key_and_nonce,20), Slice(combined_key_and_nonce,24), Slice(combined_key_and_nonce,28),
			};
		}

		public static UInt32[] ExtractNonce(byte[] combined_key_and_nonce)
		{
			return new UInt32[] {
				Slice(combined_key_and_nonce,32), Slice(combined_key_and_nonce,36), Slice(combined_key_and_nonce,40)
			};
		}

		public static byte[] SerializeKeyStream(UInt32[] key_stream)
		{
			var ret = new byte[64];
			for (int i = 0, k = 0; i < 16; ++i)
				for (int j = 0; j < 32; j += 8, ++k)
					ret[k] = (byte)((key_stream[i] >> j) & 0xFF);
			return ret;
		}

		private static void mix(UInt32[] input, UInt32[] working_state)
		{
			for (int i = 0; i < 16; ++i)
				input[i] = (input[i] + working_state[i]) & 0xFFFFFFFF;
		}

		private static void roundpair(UInt32[] state)
		{
			quarterround(state, 0, 4, 8, 12);
			quarterround(state, 1, 5, 9, 13);
			quarterround(state, 2, 6, 10, 14);
			quarterround(state, 3, 7, 11, 15);

			quarterround(state, 0, 5, 10, 15);
			quarterround(state, 1, 6, 11, 12);
			quarterround(state, 2, 7, 8, 13);
			quarterround(state, 3, 4, 9, 14);
		}

		private static void quarterround(UInt32[] state, int a, int b, int c, int d)
		{
			state[a] = (state[a] + state[b]) & 0xFFFFFFFF; state[d] ^= state[a]; state[d] = ROL(state[d], 16);
			state[c] = (state[c] + state[d]) & 0xFFFFFFFF; state[b] ^= state[c]; state[b] = ROL(state[b], 12);
			state[a] = (state[a] + state[b]) & 0xFFFFFFFF; state[d] ^= state[a]; state[d] = ROL(state[d], 8);
			state[c] = (state[c] + state[d]) & 0xFFFFFFFF; state[b] ^= state[c]; state[b] = ROL(state[b], 7);
		}

		private static UInt32 ROL(UInt32 x, int shift)
		{
			return (x << shift) | (x >> (32 - shift));
		}

		public static UInt32 Slice(byte[] b, int i)
		{
			return ((UInt32)b[i + 0] << 0) | ((UInt32)b[i + 1] << 8) | ((UInt32)b[i + 2] << 16) | ((UInt32)b[i + 3] << 24);
		}
	}
}
