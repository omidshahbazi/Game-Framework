// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.NetworkingManaged
{
	static class CallbackUtilities
	{
		public static void InvokeCallback(Action Callback)
		{
			try
			{
				Callback();
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public static void InvokeCallback<T>(Action<T> Callback, T Param1)
		{
			try
			{
				Callback(Param1);
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}