// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Deterministic
{
	public static class Math
	{
		private static readonly double[] SinTable = new double[]
		{
			0, 0.0174524064372835, 0.034899496702501, 0.0523359562429438, 0.0697564737441253, 0.0871557427476582, 0.104528463267653, 0.121869343405147, 0.139173100960065,
			0.156434465040231, 0.17364817766693, 0.190808995376545, 0.207911690817759, 0.224951054343865, 0.241921895599668, 0.258819045102521, 0.275637355816999, 0.292371704722737,
			0.309016994374947, 0.325568154457157, 0.342020143325669, 0.3583679495453, 0.374606593415912, 0.390731128489274, 0.4067366430758, 0.422618261740699, 0.438371146789077,
			0.453990499739547, 0.469471562785891, 0.484809620246337, 0.5, 0.515038074910054, 0.529919264233205, 0.544639035015027, 0.559192903470747, 0.573576436351046,
			0.587785252292473, 0.601815023152048, 0.615661475325658, 0.629320391049837, 0.642787609686539, 0.656059028990507, 0.669130606358858, 0.681998360062498, 0.694658370458997,
			0.707106781186547, 0.719339800338651, 0.73135370161917, 0.743144825477394, 0.754709580222772, 0.766044443118978, 0.777145961456971, 0.788010753606722, 0.798635510047293,
			0.809016994374947, 0.819152044288992, 0.829037572555042, 0.838670567945424, 0.848048096156426, 0.857167300702112, 0.866025403784439, 0.874619707139396, 0.882947592858927,
			0.891006524188368, 0.898794046299167, 0.90630778703665, 0.913545457642601, 0.92050485345244, 0.927183854566787, 0.933580426497202, 0.939692620785908, 0.945518575599317,
			0.951056516295154, 0.956304755963035, 0.961261695938319, 0.965925826289068, 0.970295726275996, 0.974370064785235, 0.978147600733806, 0.981627183447664, 0.984807753012208,
			0.987688340595138, 0.99026806874157, 0.992546151641322, 0.994521895368273, 0.996194698091746, 0.997564050259824, 0.998629534754574, 0.999390827019096, 0.999847695156391,
			1, 0.999847695156391, 0.999390827019096, 0.998629534754574, 0.997564050259824, 0.996194698091746, 0.994521895368273, 0.992546151641322, 0.99026806874157,
			0.987688340595138, 0.984807753012208, 0.981627183447664, 0.978147600733806, 0.974370064785235, 0.970295726275996, 0.965925826289068, 0.961261695938319, 0.956304755963036,
			0.951056516295154, 0.945518575599317, 0.939692620785908, 0.933580426497202, 0.927183854566787, 0.92050485345244, 0.913545457642601, 0.90630778703665, 0.898794046299167,
			0.891006524188368, 0.882947592858927, 0.874619707139396, 0.866025403784439, 0.857167300702112, 0.848048096156426, 0.838670567945424, 0.829037572555042, 0.819152044288992,
			0.809016994374947, 0.798635510047293, 0.788010753606722, 0.777145961456971, 0.766044443118978, 0.754709580222772, 0.743144825477394, 0.731353701619171, 0.719339800338651,
			0.707106781186548, 0.694658370458997, 0.681998360062499, 0.669130606358858, 0.656059028990507, 0.642787609686539, 0.629320391049837, 0.615661475325658, 0.601815023152048,
			0.587785252292473, 0.573576436351046, 0.559192903470747, 0.544639035015027, 0.529919264233205, 0.515038074910054, 0.5, 0.484809620246337, 0.469471562785891,
			0.453990499739547, 0.438371146789077, 0.4226182617407, 0.4067366430758, 0.390731128489274, 0.374606593415912, 0.3583679495453, 0.342020143325669, 0.325568154457157,
			0.309016994374948, 0.292371704722737, 0.275637355816999, 0.258819045102521, 0.241921895599668, 0.224951054343865, 0.207911690817759, 0.190808995376545, 0.17364817766693,
			0.156434465040231, 0.139173100960065, 0.121869343405148, 0.104528463267654, 0.0871557427476582, 0.0697564737441255, 0.0523359562429438, 0.0348994967025011, 0.0174524064372834,
			1.22460635382238E-16
		};

		public static readonly Number PI = 3.1415926535897932384626433832795;
		public static readonly Number HalfPI = PI / 2;
		public static readonly Number TwoPI = PI * 2;
		public static readonly Number ThreePI = PI * 3;
		public static readonly Number DegreesToRadians = 0.01745329251994329576923690768489;
		public static readonly Number RadiansToDegrees = 57.295779513082320876798154814105;
		public static readonly Number Epsilon = Number.Epsilon;

		public static bool AreEqual(Number Left, Number Right)
		{
			return Abs(Left - Right) <= Epsilon;
		}

		public static bool IsZero(Number Value)
		{
			return AreEqual(Value, 0);
		}

		public static Number Sqrt(Number Value)
		{
			if (Value == 0)
				return 0;

			Number n = (Value / 2) + 1;
			Number n1 = (n + (Value / n)) / 2;
			while (n1 < n)
			{
				n = n1;
				n1 = (n + (Value / n)) / 2;
			}

			return n;
		}

		public static Number Sin(Number Value)
		{
			Value *= RadiansToDegrees;
			Value %= 360;

			if (Value < 0)
				Value += 360;

			int coef = 1;
			int index = (int)Value;
			Number precision = Value - (int)Value;

			if (Value > 180)
			{
				index = (int)Value - 180;
				coef = -1;
			}

			if (precision <= Math.Epsilon)
				return SinTable[index] * coef;

			return Lerp(SinTable[index], SinTable[index + 1], precision) * coef;
		}

		public static Number Cos(Number Value)
		{
			return Sin(Value + 1.570796F);
		}

		public static Number Tan(Number Value)
		{
			return Sin(Value) / Cos(Value);
		}

		public static Number Asin(Number Value)
		{
			int coef = 1;
			if (Value < 0)
			{
				Value *= -1;
				coef = -1;
			}

			for (int i = 1; i < SinTable.Length; ++i)
			{
				if (SinTable[i] < Value)
					continue;

				Number diff = SinTable[i] - SinTable[i - 1];

				return ((i - 1) + (diff == 0 ? (Number)0 : (Value - SinTable[i - 1]) / diff)) * coef * DegreesToRadians;
			}

			return 0;
		}

		public static Number Atan(Number Value)
		{
			return Asin(Value / Sqrt(1 + (Value * Value)));
		}

		public static Number Atan2(Number Value1, Number Value2)
		{
			if (Value1 == 0 && Value2 == 0)
				return 0;

			Number result = 0;

			if (Value2 > 0)
				result = Atan(Value1 / Value2);
			else if (Value2 < 0)
				result = PI - Atan(Abs(Value1 / Value2));
			else
				result = PI / 2;

			result *= (Value1 >= 0 ? 1 : -1);

			return result;
		}

		public static Number Abs(Number Value)
		{
			if (Value < 0)
				return -(float)Value;
			else
				return Value;
		}

		public static Number Floor(Number Value)
		{
			if ((int)Value == 0)
				return 0;

			return (Value - (Value % (int)Value));
		}

		public static Number Ceil(Number Value)
		{
			Number floor = Floor(Value);

			return (Value - floor == 0 ? Value : Floor(Value) + 1);
		}

		public static int Round(Number Value)
		{
			Number frac = Value % (int)Value;

			int value = (int)Value;

			if (frac < 0.5F)
				return value;

			return value + 1;
		}

		public static Number Clamp01(Number Value)
		{
			return Clamp(Value, 0, 1);
		}

		public static Number Clamp(Number Value, Number Min, Number Max)
		{
			return (Value < Min ? Min : Value > Max ? Max : Value);
		}

		public static Number Lerp(Number Min, Number Max, Number Time)
		{
			if (Time < 0.5F)
				return (Min + (Max - Min) * Clamp01(Time));

			return (Max - (Max - Min) * Clamp01(1 - Time));
		}

		public static Number LerpUnclamped(Number Min, Number Max, Number Time)
		{
			if (Time < 0.5F)
				return (Min + (Max - Min) * Time);

			return (Max - (Max - Min) * (1 - Time));
		}

		public static Number Min(params Number[] Values)
		{
			Number result = Number.MaxValue;

			for (int i = 0; i < Values.Length; ++i)
				if (result > Values[i])
					result = Values[i];

			return result;
		}

		public static Number Max(params Number[] Values)
		{
			Number result = Number.MinValue;

			for (int i = 0; i < Values.Length; ++i)
				if (result < Values[i])
					result = Values[i];

			return result;
		}

		public static Number Pow(Number Value1, int Power)
		{
			Number result = Value1;

			for (int i = 1; i < Power; ++i)
				result *= Value1;

			return result;
		}

		public static int Sign(Number Value)
		{
			if (Value < 0)
				return -1;

			if (Value > 0)
				return 1;

			return 0;
		}

		public static bool BiasGreaterThan(Number A, Number B)
		{
			Number BiasRelative = 0.95f;
			Number BiasAbsolute = 0.01f;

			return A >= B * BiasRelative + A * BiasAbsolute;
		}
	}
}
