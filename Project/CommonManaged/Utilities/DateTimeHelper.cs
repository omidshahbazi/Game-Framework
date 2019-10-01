// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Common.Utilities
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
