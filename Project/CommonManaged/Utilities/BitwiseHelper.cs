// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
namespace Zorvan.Framework.Common.Utilities
{
	public static class BitwiseHelper
	{
		public static int Enable(int Mask, int Bit)
		{
			return (Mask | Bit);
		}
		public static long Enable(long Mask, long Bit)
		{
			return (Mask | Bit);
		}

		public static int Disable(int Mask, int Bit)
		{
			return Mask ^ (Mask & Bit);
		}
		public static long Disable(long Mask, long Bit)
		{
			return Mask ^ (Mask & Bit);
		}

		public static int Toggle(int Mask, int Bit)
		{
			return (Mask ^ Bit);
		}
		public static long Toggle(long Mask, long Bit)
		{
			return (Mask ^ Bit);
		}

		public static bool IsEnabled(int Mask, int Bit)
		{
			return (Mask == Bit || (Mask & Bit) != 0);
		}
		public static bool IsEnabled(long Mask, long Bit)
		{
			return (Mask == Bit || (Mask & Bit) != 0);
		}
	}
}