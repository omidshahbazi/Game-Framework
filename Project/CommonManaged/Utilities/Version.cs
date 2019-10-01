// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Common.Utilities
{
	public struct Version
	{
		private uint value;

		public uint Major
		{
			get { return value >> 24; }
			set { this.value = ((value & 0xFF) << 24) | ((Minor & 0xFF) << 16) | ((Build & 0xFF) << 8) | (Revision & 0xFF); }
		}

		public uint Minor
		{
			get { return (value >> 16) & 0xFF; }
			set { this.value = ((Major & 0xFF) << 24) | ((value & 0xFF) << 16) | ((Build & 0xFF) << 8) | (Revision & 0xFF); }
		}

		public uint Build
		{
			get { return (value >> 8) & 0xFF; }
			set { this.value = ((Major & 0xFF) << 24) | ((Minor & 0xFF) << 16) | ((value & 0xFF) << 8) | (Revision & 0xFF); }
		}

		public uint Revision
		{
			get { return value & 0xFF; }
			set { this.value = ((Major & 0xFF) << 24) | ((Minor & 0xFF) << 16) | ((Build & 0xFF) << 8) | (value & 0xFF); }
		}

		public uint Value
		{
			get { return value; }
		}

		public Version(uint Value)
		{
			value = Convert.ToUInt32(Value);
		}

		public Version(uint Major, uint Minor, uint Build, uint Revision)
		{
			value = ((Major & 0xFF) << 24) | ((Minor & 0xFF) << 16) | ((Build & 0xFF) << 8) | (Revision & 0xFF);
		}

		public Version(string Value)
		{
			string[] parts = Value.Split('.');

			value = 0;

			int index = 0;
			if (index < parts.Length)
				value = (Convert.ToUInt32(parts[index++]) & 0xFF) << 24;
			if (index < parts.Length)
				value |= (Convert.ToUInt32(parts[index++]) & 0xFF) << 16;
			if (index < parts.Length)
				value |= (Convert.ToUInt32(parts[index++]) & 0xFF) << 8;
			if (index < parts.Length)
				value |= Convert.ToUInt32(parts[index]) & 0xFF;
		}

		public static bool operator ==(Version Left, Version Right)
		{
			return (Left.Value == Right.Value);
		}

		public static bool operator !=(Version Left, Version Right)
		{
			return !(Left == Right);
		}

		public static bool operator <(Version Left, Version Right)
		{
			return (Left.Value < Right.Value);
		}

		public static bool operator <=(Version Left, Version Right)
		{
			return (Left.Value <= Right.Value);
		}

		public static bool operator >(Version Left, Version Right)
		{
			return (Left.Value > Right.Value);
		}

		public static bool operator >=(Version Left, Version Right)
		{
			return (Left.Value >= Right.Value);
		}

		public static Version operator ++(Version Left)
		{
			if (Left.Revision < 255)
				Left.Revision++;
			else if (Left.Build < 255)
				Left.Build++;
			else if (Left.Minor < 255)
				Left.Minor++;
			else if (Left.Major < 255)
				Left.Major++;

			return Left;
		}

		public override bool Equals(object obj)
		{
			if (obj is Version)
				return (this == (Version)obj);

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return Major + "." + Minor + "." + Build + "." + Revision;
		}
	}
}