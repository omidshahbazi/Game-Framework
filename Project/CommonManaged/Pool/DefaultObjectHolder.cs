// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System.Collections.Generic;

namespace Zorvan.Framework.Common.Pool
{
	class DefaultObjectHolder<T> : IObjectHolder<T> where T : class, IObject
	{
		private List<T> freeObjects = new List<T>();

		public T Pull(object UserData)
		{
			if (freeObjects.Count == 0)
				return null;

			T obj = freeObjects[0];
			freeObjects.RemoveAt(0);
			return obj;
		}

		public void Push(T Object)
		{
			freeObjects.Add(Object);
		}

		public void Clear()
		{
			freeObjects.Clear();
		}
	}
}