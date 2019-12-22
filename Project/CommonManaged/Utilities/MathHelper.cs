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
	}
}