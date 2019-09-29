// Copyright 2019. All Rights Reserved.
using System;

namespace Zorvan.Framework.Common.Utilities
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
	}
}