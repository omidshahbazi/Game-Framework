// Copyright 2019. All Rights Reserved.

namespace GameFramework.Deterministic
{
	public struct Number
	{
		public static readonly Number MaxValue = 99999;
		public static readonly Number MinValue = -99999;
		public static readonly Number Epsilon = 1.401298E-45F;

#if FIXED_POINT_MATH
		private const int SHIFT_AMOUNT = 12; //12 is 4096
		private const long One = 1 << SHIFT_AMOUNT;

		private long value;

		private float FloatValue
		{
			get { return value / (float)One; }
		}

		private double DoubleValue
		{
			get { return value / (double)One; }
		}

		public long RawValue
		{
			get { return value; }
		}

		public Number(long RawValue)
		{
			value = RawValue;
		}

		private Number(float Value)
		{
			Value *= One;
			value = (int)Value;
		}

		private Number(double Value)
		{
			Value *= One;
			value = (int)Value;
		}

		public static Number operator *(Number LeftHand, Number RightHand)
		{
			return new Number((LeftHand.value * RightHand.value) >> SHIFT_AMOUNT);
		}

		public static Number operator /(Number LeftHand, Number RightHand)
		{
			return new Number((LeftHand.value << SHIFT_AMOUNT) / RightHand.value);
		}

		public static Number operator %(Number LeftHand, Number RightHand)
		{
			return new Number(LeftHand.value % RightHand.value);
		}

		public static implicit operator float(Number Value)
		{
			return Value.FloatValue;
		}

		public static implicit operator double(Number Value)
		{
			return Value.DoubleValue;
		}

		public override string ToString()
		{
			return FloatValue.ToString();
		}
#else
		private double value;

		private Number(float Value)
		{
			value = Value;
		}

		private Number(double Value)
		{
			value = Value;
		}

		public static Number operator *(Number LeftHand, Number RightHand)
		{
			return new Number(LeftHand.value * RightHand.value);
		}

		public static Number operator /(Number LeftHand, Number RightHand)
		{
			return new Number(LeftHand.value / RightHand.value);
		}

		public static Number operator %(Number LeftHand, Number RightHand)
		{
			return new Number(LeftHand.value % RightHand.value);
		}

		public static implicit operator float(Number Value)
		{
			return (float)Value.value;
		}

		public static implicit operator double(Number Value)
		{
			return Value.value;
		}

		public override string ToString()
		{
			return value.ToString();
		}
#endif

		public static Number operator +(Number LeftHand, Number RightHand)
		{
			return new Number(LeftHand.value + RightHand.value);
		}

		public static Number operator -(Number LeftHand, Number RightHand)
		{
			return new Number(LeftHand.value - RightHand.value);
		}

		public static bool operator ==(Number LeftHand, Number RightHand)
		{
			return LeftHand.value == RightHand.value;
		}

		public static bool operator !=(Number LeftHand, Number RightHand)
		{
			return LeftHand.value != RightHand.value;
		}

		public static bool operator >(Number LeftHand, Number RightHand)
		{
			return LeftHand.value > RightHand.value;
		}

		public static bool operator >=(Number LeftHand, Number RightHand)
		{
			return LeftHand.value >= RightHand.value;
		}

		public static bool operator <(Number LeftHand, Number RightHand)
		{
			return LeftHand.value < RightHand.value;
		}

		public static bool operator <=(Number LeftHand, Number RightHand)
		{
			return LeftHand.value <= RightHand.value;
		}

		public static implicit operator Number(float Value)
		{
			return new Number(Value);
		}

		public static implicit operator Number(double Value)
		{
			return new Number((float)Value);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
	}
}
