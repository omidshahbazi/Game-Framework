// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;

namespace Zorvan.Framework.Common.Timing
{
	public class Time
	{
		public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH-mm";

		public static DateTime BeginingOfEpochDateTime
		{
			get { return new DateTime(1970, 1, 1, 0, 0, 0, 0); }
		}

		public static DateTime CurrentUTCDateTime
		{
			get { return DateTime.UtcNow; }
		}

		public static double CurrentEpochTime
		{
			get { return CurrentUTCDateTime.Subtract(BeginingOfEpochDateTime).TotalSeconds; }
		}

		public static double YesterdayTime
		{
			get
			{
				return new DateTime(CurrentUTCDateTime.Year, CurrentUTCDateTime.Month, CurrentUTCDateTime.Day - 1).Subtract(BeginingOfEpochDateTime).TotalSeconds;
			}
		}

		public static double TodayTime
		{
			get
			{
				return new DateTime(CurrentUTCDateTime.Year, CurrentUTCDateTime.Month, CurrentUTCDateTime.Day).Subtract(BeginingOfEpochDateTime).TotalSeconds;
			}
		}

		public static double TomorrowTime
		{
			get
			{
				return new DateTime(CurrentUTCDateTime.Year, CurrentUTCDateTime.Month, CurrentUTCDateTime.Day + 1).Subtract(BeginingOfEpochDateTime).TotalSeconds;
			}
		}

		public static string FormattedDateTime
		{
			get { return CurrentUTCDateTime.ToString(DATE_TIME_FORMAT); }
		}
	}
}
