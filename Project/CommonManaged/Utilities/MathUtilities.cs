// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;

namespace GameFramework.Common.Utilities
{
	public static class MathUtilities
	{
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
