// Copyright 2019. All Rights Reserved.
using System.Runtime.CompilerServices;

namespace GameFramework.Deterministic
{
	public struct Number
	{
		private const long ONE = 1 << SHIFT_AMOUNT;
		private const int SHIFT_AMOUNT = 12;

		[CompilerGenerated]
		private long RawValue;

		public float Value
		{
			get { return RawValue / (float)ONE; }
			set
			{
				value *= ONE;
				RawValue = (int)System.Math.Round(value);
			}
		}

		private Number(long Value)
		{
			RawValue = Value;
		}

		public Number(float Value)
		{
			Value *= ONE;
			RawValue = (int)System.Math.Round(Value);
		}

		public static Number operator *(Number Left, Number Right)
		{
			return new Number((Left.RawValue * Right.RawValue) >> SHIFT_AMOUNT);
		}

		public static Number operator /(Number Left, Number Right)
		{
			return new Number((Left.RawValue << SHIFT_AMOUNT) / (Right.RawValue));
		}

		public static Number operator %(Number Left, Number Right)
		{
			return new Number((Left.RawValue) % (Right.RawValue));
		}

		public static Number operator +(Number Left, Number Right)
		{
			return new Number(Left.RawValue + Right.RawValue);
		}

		public static Number operator -(Number Left, Number Right)
		{
			return new Number(Left.RawValue - Right.RawValue);
		}

		public static bool operator ==(Number Left, Number Right)
		{
			return Left.RawValue == Right.RawValue;
		}

		public static bool operator !=(Number Left, Number Right)
		{
			return Left.RawValue != Right.RawValue;
		}

		public static bool operator >(Number Left, Number Right)
		{
			return Left.RawValue > Right.RawValue;
		}

		public static bool operator <(Number Left, Number Right)
		{
			return Left.RawValue < Right.RawValue;
		}

		public static implicit operator int(Number Other)
		{
			return (int)Other.Value;
		}

		public static implicit operator float(Number Other)
		{
			return Other.Value;
		}

		public static implicit operator double(Number Other)
		{
			return Other.Value;
		}

		public static implicit operator Number(float Other)
		{
			return new Number(Other);
		}

		public static implicit operator Number(double Other)
		{
			return new Number((float)Other);
		}

		public static implicit operator Number(int Other)
		{
			return new Number(Other << SHIFT_AMOUNT);
		}

		public static implicit operator Number(long Other)
		{
			return new Number(Other << SHIFT_AMOUNT);
		}

		public static implicit operator Number(ulong Other)
		{
			return new Number((long)Other << SHIFT_AMOUNT);
		}

		public override bool Equals(object obj)
		{
			if (obj is Number)
				return ((Number)obj).RawValue == RawValue;
			else
				return false;
		}

		public override string ToString()
		{
			return "N[" + Value + "]";
		}
	}
}
