﻿// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Networking
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

		public static void InvokeCallback<P1>(Action<P1> Callback, P1 Param1)
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

		public static void InvokeCallback<P1, P2>(Action<P1, P2> Callback, P1 Param1, P2 Param2)
		{
			try
			{
				Callback(Param1, Param2);
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}