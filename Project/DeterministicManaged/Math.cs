// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Deterministic
{
	public static class Math
	{
		private static int[] SIN_TABLE = {
			0, 71, 142, 214, 285, 357, 428, 499, 570, 641,
			711, 781, 851, 921, 990, 1060, 1128, 1197, 1265, 1333,
			1400, 1468, 1534, 1600, 1665, 1730, 1795, 1859, 1922, 1985,
			2048, 2109, 2170, 2230, 2290, 2349, 2407, 2464, 2521, 2577,
			2632, 2686, 2740, 2793, 2845, 2896, 2946, 2995, 3043, 3091,
			3137, 3183, 3227, 3271, 3313, 3355, 3395, 3434, 3473, 3510,
			3547, 3582, 3616, 3649, 3681, 3712, 3741, 3770, 3797, 3823,
			3849, 3872, 3895, 3917, 3937, 3956, 3974, 3991, 4006, 4020,
			4033, 4045, 4056, 4065, 4073, 4080, 4086, 4090, 4093, 4095,
			4096
		};

		public static Number PI = new Number(3.1415926535897932384626433832795F);
		public static Number TwoPI = PI * 2;

		public static Number Sqrt(Number Value)
		{
			return System.Math.Sqrt(Value.Value);
		}

		//private static Number SinLookup(Number Value, Number j)
		//{
		//	if (j > 0 && j < new Number(10, false) && i < new Number(90, false))
		//		return new Number(SIN_TABLE[i.RawValue], false) +
		//			((new Number(SIN_TABLE[i.RawValue + 1], false) - new Number(SIN_TABLE[i.RawValue], false)) /
		//			new Number(10, false)) * j;
		//	else
		//		return new Number(SIN_TABLE[i.RawValue], false);
		//}

		public static Number Sin(Number Value)
		{
			//Number j = (Number)0;
			//for (; i < 0; i += new Number(25736, false)) ;
			//if (i > new Number(25736, false))
			//	i %= new Number(25736, false);
			//Number k = (i * new Number(10, false)) / new Number(714, false);
			//if (i != 0 && i != new Number(6434, false) && i != new Number(12868, false) &&
			//	i != new Number(19302, false) && i != new Number(25736, false))
			//	j = (i * new Number(100, false)) / new Number(714, false) - k * new Number(10, false);
			//if (k <= new Number(90, false))
			//	return SinLookup(k, j);
			//if (k <= new Number(180, false))
			//	return SinLookup(new Number(180, false) - k, j);
			//if (k <= new Number(270, false))
			//	return SinLookup(k - new Number(180, false), j).Inverse;
			//else
			//	return SinLookup(new Number(360, false) - k, j).Inverse;

			return System.Math.Sin(Value.Value);
		}

		public static Number Cos(Number Value)
		{
			return Sin(Value.Value + 1.570796F);
		}

		public static Number Tan(Number Value)
		{
			return Sin(Value.Value) / Cos(Value.Value);
		}

		public static Number Asin(Number Value)
		{
			//bool isNegative = Value < 0;

			//Value = Abs(Value);

			//if (Value > 1)
			//	throw new ArithmeticException("Bad Asin Input:" + Value.ToDouble());

			//Number f1 = mul(mul(mul(mul(new Number(145103 >> Number.SHIFT_AMOUNT, false), F) -
			//	new Number(599880 >> Number.SHIFT_AMOUNT, false), F) +
			//	new Number(1420468 >> Number.SHIFT_AMOUNT, false), F) -
			//	new Number(3592413 >> Number.SHIFT_AMOUNT, false), F) +
			//	new Number(26353447 >> Number.SHIFT_AMOUNT, false);
			//Number f2 = PI / new Number(2, true) - (Sqrt(Number.OneF - F) * f1);

			//return isNegative ? f2.Inverse : f2;

			return System.Math.Asin(Value.Value);
		}

		public static Number Atan(Number Value)
		{
			return Asin(Value / Sqrt(1 + (Value.Value * Value.Value)));
		}

		public static Number Atan2(Number Value1, Number Value2)
		{
			//if (Value1 == 0 && Value2 == 0)
			//	return 0;

			//Number result = 0;
			//if (Value2 > 0)
			//	result = Atan(Value1 / Value2);
			//else if (Value2 < 0)
			//{
			//	if (Value1 >= 0)
			//		result = (PI - Atan(Abs(Value1 / Value2)));
			//	else
			//		result = (PI - Atan(Abs(Value1 / Value2))).Inverse;
			//}
			//else
			//	result = (Value1 >= 0 ? PI : (Number)(-(float)PI)) / 2;

			//return result;

			return System.Math.Atan2(Value1, Value2);
		}

		public static Number Abs(Number Value)
		{
			if (Value < 0)
				return -(float)Value;
			else
				return Value;
		}

		public static int Round(Number Value)
		{
			Number frac = Value % (int)Value;

			int value = (int)Value;

			if (frac < 0.5F)
				return value;

			return value + 1;
		}

		public static Number Floor(Number Value)
		{
			return System.Math.Floor(Value.Value);
		}

		public static Number Ceil(Number Value)
		{
			return System.Math.Floor(Value.Value);
		}

		public static Number Min(params Number[] Values)
		{
			Number result = float.MaxValue;

			for (int i = 0; i < Values.Length; ++i)
				if (result > Values[i].Value)
					result = Values[i].Value;

			return result;
		}

		public static Number Max(params Number[] Values)
		{
			Number result = float.MinValue;

			for (int i = 0; i < Values.Length; ++i)
				if (result < Values[i].Value)
					result = Values[i].Value;

			return result;
		}

		public static Number Pow(Number Value1, int Power)
		{
			Number result = Value1;

			for (int i = 1; i < Power; ++i)
				result *= Value1.Value;

			return result;
		}

		public static int Sign(Number Value)
		{
			if (Value.Value < 0)
				return -1;

			if (Value > 0)
				return 1;

			return 0;
		}
	}
}
