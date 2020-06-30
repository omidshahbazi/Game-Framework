// Copyright 2019. All Rights Reserved.
namespace GameFramework.Deterministic
{
	public struct Vector3
	{
		public static readonly Vector3 Zero = new Vector3(0, 0, 0);
		public static readonly Vector3 One = new Vector3(1, 1, 1);
		public static readonly Vector3 Up = new Vector3(0, 1, 0);
		public static readonly Vector3 Right = new Vector3(1, 0, 0);
		public static readonly Vector3 Forward = new Vector3(0, 0, 1);

		public Number X;
		public Number Y;
		public Number Z;

		public Number SqrMagnitude
		{
			get { return (X * X) + (Y * Y) + (Z * Z); }
		}

		public Number Magnitude
		{
			get { return Math.Sqrt(SqrMagnitude); }
		}

		public Vector3 Normalized
		{
			get
			{
				Vector3 value = this;
				value.Normalize();
				return value;
			}
		}

		public Vector3(Number X, Number Y, Number Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}

		public void Normalize()
		{
			Number value = Magnitude;

			if (Math.IsZero(value))
				return;

			X /= value;
			Y /= value;
			Z /= value;
		}

		public Number Dot(Vector3 Other)
		{
			return (X * Other.X) + (Y * Other.Y) + (Z * Other.Z);
		}

		public Vector3 Cross(Vector3 Other)
		{
			return new Vector3((Y * Other.Z) - (Z * Other.Y), (Z * Other.X) - (X * Other.Z), (X * Other.Y) - (Y * Other.X));
		}

		public static Vector3 operator -(Vector3 Other)
		{
			return new Vector3(-Other.X, -Other.Y, -Other.Z);
		}

		public static Vector3 operator *(Vector3 Left, Number Right)
		{
			return new Vector3(Left.X * Right, Left.Y * Right, Left.Z * Right);
		}

		public static Vector3 operator /(Vector3 Left, Number Right)
		{
			return new Vector3(Left.X / Right, Left.Y / Right, Left.Z / Right);
		}

		public static Vector3 operator /(Vector3 Left, Vector3 Right)
		{
			return new Vector3(Left.X / Right.X, Left.Y / Right.Y, Left.Z / Right.Z);
		}

		public static Vector3 operator +(Vector3 Left, Number Right)
		{
			return new Vector3(Left.X + Right, Left.Y + Right, Left.Z + Right);
		}

		public static Vector3 operator +(Vector3 Left, Vector3 Right)
		{
			return new Vector3(Left.X + Right.X, Left.Y + Right.Y, Left.Z + Right.Z);
		}

		public static Vector3 operator -(Vector3 Left, Vector3 Right)
		{
			return new Vector3(Left.X - Right.X, Left.Y - Right.Y, Left.Z - Right.Z);
		}

		public static bool operator ==(Vector3 Left, Vector3 Right)
		{
			return (Left.X == Right.X && Left.Y == Right.Y && Left.Z == Right.Z);
		}

		public static bool operator !=(Vector3 Left, Vector3 Right)
		{
			return !(Left == Right);
		}

		public static bool operator >(Vector3 Left, Vector3 Right)
		{
			return (Left.X > Right.X && Left.Y > Right.Y && Left.Z > Right.Z);
		}

		public static bool operator >=(Vector3 Left, Vector3 Right)
		{
			return (Left.X >= Right.X && Left.Y >= Right.Y && Left.Z >= Right.Z);
		}

		public static bool operator <(Vector3 Left, Vector3 Right)
		{
			return (Left.X < Right.X && Left.Y < Right.Y && Left.Z < Right.Z);
		}

		public static bool operator <=(Vector3 Left, Vector3 Right)
		{
			return (Left.X <= Right.X && Left.Y <= Right.Y && Left.Z <= Right.Z);
		}

		public override string ToString()
		{
			return "(" + X + ", " + Y + ", " + Z + ")";
		}
	}
}