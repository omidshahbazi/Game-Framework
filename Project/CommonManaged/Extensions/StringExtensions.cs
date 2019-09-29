// Copyright 2019. All Rights Reserved.
using System;

namespace Zorvan.Framework.Common.Extensions
{
	public static class StringExtensions
	{
		public static string SeparateWords(this string a)
		{
			if (string.IsNullOrEmpty(a))
				return a;

			int len = a.Length;

			string title = "";

			char c = a[0];
			title += char.IsLower(c) ? char.ToUpper(c) : c;

			for (int i = 1; i < len; i++)
			{
				c = a[i];

				if (char.IsUpper(c) && char.IsLower(a[i - 1]))
					title += ' ';

				title += c;
			}

			return title;
		}

		public static bool Contains(this string[] a, string Value)
		{
			if (a != null)
				for (int i = 0; i < a.Length; i++)
					if (a[i] == Value)
						return true;

			return false;
		}

		public static string FormatTime(double Time)
		{
			if (Time <= 0)
				return "00:00:00";

			TimeSpan timeSpan = TimeSpan.FromSeconds(Time);

			return string.Format("{0:D2}:{1:D2}:{2:D2}",
				timeSpan.Hours,
				timeSpan.Minutes,
				timeSpan.Seconds);
		}

		public static string FormatDateTime(double Time)
		{
			if (Time <= 0)
				return "00d 00h 00m";

			TimeSpan timeSpan = TimeSpan.FromSeconds(Time);

			return string.Format("{0:D2}d {1:D2}hr {2:D2}m",
				timeSpan.Days,
				timeSpan.Hours,
				timeSpan.Minutes);
		}
	}
}