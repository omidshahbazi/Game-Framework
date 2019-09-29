// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;

namespace Zorvan.Framework.Common.Utilities
{
	public static class DateTimeHelper
	{
		public static DateTime UTCDate
		{
			get { return DateTime.UtcNow; }
		}

		public static float UTCTotalSeconds
		{
			get { return (float)(UTCDate - new DateTime(1970, 1, 1)).TotalSeconds; }
		}

		public static double Time
		{
			get { return UTCDate.ToOADate() * 86400.0D; }
		}
	}
}
