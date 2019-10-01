// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Common.MemoryManagement
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