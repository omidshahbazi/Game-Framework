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
			get { return Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2); }
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
			if (Math.IsZero(X) && Math.IsZero(Y) && Math.IsZero(Z))
				return;

			Number value = Magnitude;

			X /= value;
			Y /= value;
			Z /= value;
		}

		public Number Dot(Vector3 Other)
		{
			return (X * Other.X) + (Y * Other.Y) + (Z * Other.Z);
		}

		public static Vector3 operator -(Vector3 Other)
		{
			return new Vector3(-Other.X, -Other.Y, -Other.Z);
		}

		public static Vector3 operator *(Vector3 Left, Number Right)
		{
			return new Vector3(Left.X * Right, Left.Y * Right, Left.Z * Right);
		}

		public static Vector3 operator *(Vector3 Left, Vector3 Right)
		{
			return new Vector3((Left.Y * Right.Z) - (Left.Z * Right.Y), (Left.X * Right.Z) - (Left.Z * Right.X), (Left.X * Right.Y) - (Left.Z * Right.X));
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