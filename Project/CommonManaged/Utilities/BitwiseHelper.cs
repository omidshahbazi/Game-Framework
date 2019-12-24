// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Common.Utilities
{
	public static class BitwiseHelper
	{
		public static int Enable(int Mask, ushort BitIndex)
		{
			return (int)(Enable((long)Mask, BitIndex));
		}
		public static uint Enable(uint Mask, ushort BitIndex)
		{
			return (uint)(Enable((long)Mask, BitIndex));
		}
		public static long Enable(long Mask, ushort BitIndex)
		{
			return (Mask | (1L << BitIndex));
		}

		public static int Disable(int Mask, ushort BitIndex)
		{
			return (int)(Disable((long)Mask, BitIndex));
		}
		public static uint Disable(uint Mask, ushort BitIndex)
		{
			return (uint)(Disable((long)Mask, BitIndex));
		}
		public static long Disable(long Mask, ushort BitIndex)
		{
			return (Mask & (1L << BitIndex));
		}

		public static int Toggle(int Mask, ushort BitIndex)
		{
			return (int)(Toggle((long)Mask, BitIndex));
		}
		public static uint Toggle(uint Mask, ushort BitIndex)
		{
			return (uint)(Toggle((long)Mask, BitIndex));
		}
		public static long Toggle(long Mask, ushort BitIndex)
		{
			return (Mask ^ (1L << BitIndex));
		}

		public static bool IsEnabled(int Mask, ushort BitIndex)
		{
			return IsEnabled((long)Mask, BitIndex);
		}
		public static bool IsEnabled(uint Mask, ushort BitIndex)
		{
			return IsEnabled((long)Mask, BitIndex);
		}
		public static bool IsEnabled(long Mask, ushort BitIndex)
		{
			return ((Mask & (1L << BitIndex)) != 0);
		}

		public static byte[] GetBits(uint Value)
		{
			ushort bitCount = sizeof(uint) * 8;

			byte[] result = new byte[bitCount];
			for (ushort i = 0; i < bitCount; ++i)
				result[i] = (byte)((Value & (1L << i)) == 0 ? 0 : 1);

			return result;
		}

		public static unsafe byte[] GetBytes(object Value, Type Type)
		{
			int size = (int)Type.GetSizeOf();
			byte[] buffer = new byte[size];

			TypedReference valueRef = __makeref(Value);
			byte* valuePtr = (byte*)*((IntPtr*)(&valueRef));

			for (int i = 0; i < size; ++i)
				buffer[i] = valuePtr[i];

			return buffer;
		}

		public static T GetObject<T>(byte[] Buffer)
		{
			return (T)GetObject(typeof(T), Buffer);
		}

		public static unsafe object GetObject(Type Type, byte[] Buffer)
		{
			uint size = Type.GetSizeOf();
			object value = Activator.CreateInstance(Type);

			TypedReference valueRef = __makeref(value);
			byte* valuePtr = (byte*)*((IntPtr*)(&valueRef));

			for (int i = 0; i < size; ++i)
				valuePtr[i] = Buffer[i];

			return value;
		}
	}
}