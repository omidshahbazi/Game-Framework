// Copyright 2019. All Rights Reserved.
namespace GameFramework.Common.Utilities
{
	public struct OUID
	{
		public static readonly OUID Empty = new OUID(string.Empty);

		private string value;

		public string Value
		{
			get { return value; }
		}

		public OUID(string Value)
		{
			value = Value;
		}

		public static implicit operator OUID(string Value)
		{
			return new OUID(Value);
		}

		public static bool operator ==(OUID Left, OUID Right)
		{
			return (Left.Value == Right.Value);
		}

		public static bool operator !=(OUID Left, OUID Right)
		{
			return !(Left == Right);
		}

		public override bool Equals(object obj)
		{
			if (obj is OUID)
				return (this == (OUID)obj);

			return false;
		}

		public override int GetHashCode()
		{
			return (int)CRC32.CalculateHash(System.Text.Encoding.ASCII.GetBytes(Value));
		}

		public override string ToString()
		{
			return Value;
		}

		public static OUID Make()
		{
			return new OUID(System.Guid.NewGuid().ToString("D"));
		}
	}
}