// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;
using System.Diagnostics;
using GameFramework.Common.Pool;

namespace GameFramework.Common.Extensions
{
	public static class ListExtensions
	{
		public static void Resize<T, U>(this List<T> a, int NewSize, U Pool)
			where T : class, IObject
			where U : ObjectPool<T>
		{
			int diff = NewSize - a.Count;

			if (diff > 0)
			{
				for (int i = 0; i < diff; ++i)
					a.Add(Pool.Pull(null));
			}
			else if (diff < 0)
			{
				for (int i = a.Count - 1; i >= NewSize; --i)
					Pool.Push(a[i]);
				a.RemoveRange(NewSize, -diff);
			}

			Debug.Assert(a.Count == NewSize);
		}

		public static void Resize<T>(this List<T> a, int NewSize)
			where T : class, IObject
		{
			a.Resize(NewSize, ObjectPoolSingleton<T>.Instance);
		}
	}
}