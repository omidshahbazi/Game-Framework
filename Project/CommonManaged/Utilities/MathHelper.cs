// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;

namespace GameFramework.Common.Utilities
{
	public static class MathHelper
	{
		private const float MIDDLE_OF_INTEGER = 1.0F / 2.0F;

		public static float GetClosestFactorOf(float Value, float Multiplier)
		{
			return ((int)(Value / Multiplier) + (Value % Multiplier > MIDDLE_OF_INTEGER ? 1.0F : 0)) * Multiplier;
		}

		public static int Round(float Value, RoundBehaviors Behavior)
		{
			switch (Behavior)
			{
				case RoundBehaviors.AlwaysFloor:
					return (int)Math.Floor(Value);
				case RoundBehaviors.AlwaysCeil:
					return (int)Math.Ceiling(Value);
				case RoundBehaviors.ClosestAndMiddleToFloor:
				case RoundBehaviors.ClosestAndMiddleToCeil:
					{
						int valueAsInt = (int)Math.Floor(Value);
						float remain = Value % valueAsInt;

						if (remain == 0)
							return valueAsInt;

						if (remain == MIDDLE_OF_INTEGER)
						{
							if (Behavior == RoundBehaviors.ClosestAndMiddleToFloor)
								return valueAsInt;
							else
								return valueAsInt + 1;
						}

						if (remain < MIDDLE_OF_INTEGER)
							return valueAsInt;

						return valueAsInt + 1;
					}
			}

			return 0;
		}

		public static T Min<T>(IEnumerable<T> Collection) where T : struct, IComparable<T>
		{
			var it = Collection.GetEnumerator();

			if (!it.MoveNext())
				return default(T);

			T min = it.Current;

			while (it.MoveNext())
				if (min.CompareTo(it.Current) > 0)
					min = it.Current;

			return min;
		}

		public static T Max<T>(IEnumerable<T> Collection) where T : struct, IComparable<T>
		{
			var it = Collection.GetEnumerator();

			if (!it.MoveNext())
				return default(T);

			T max = it.Current;

			while (it.MoveNext())
				if (max.CompareTo(it.Current) < 0)
					max = it.Current;

			return max;
		}

		public static float Average(IEnumerable<float> Collection)
		{
			float sum = 0.0F;
			int count = 0;

			var it = Collection.GetEnumerator();

			while (it.MoveNext())
			{
				sum += it.Current;

				++count;
			}

			if (count == 0)
				return 0;

			return (sum / count);
		}

		public static T Clamp<T>(T Value, T Min, T Max) where T : struct, IComparable<T>
		{
			if (Value.CompareTo(Min) < 0)
				return Min;

			if (Value.CompareTo(Max) > 0)
				return Max;

			return Value;
		}

		public static int[] FindMultiplesOf(int Value)
		{
			int halfValue = Value / 2;

			List<int> multiples = new List<int>();

			for (int i = 2; i <= halfValue; ++i)
				if (Value % i == 0)
					multiples.Add(i);

			return multiples.ToArray();
		}
	}
}