// Copyright 2019. All Rights Reserved.
using System.Drawing;

namespace GameFramework.Common.Extensions
{
	public static class ColorExtensions
	{
		public static string ToHex(this Color Self)
		{
			return "#" + Self.R.ToString("X2") + Self.G.ToString("X2") + Self.B.ToString("X2");
		}
	}
}