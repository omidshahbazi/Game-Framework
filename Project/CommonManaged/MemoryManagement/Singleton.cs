// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;

namespace Zorvan.Framework.Common.MemoryManagement
{
	public class Singleton<T> where T : class
	{
		private static T instance = null;

		public static T Instance
		{
			get
			{
				if (instance == null)
					instance = (T)Activator.CreateInstance(typeof(T), true);

				return instance;
			}
		}
	}
}