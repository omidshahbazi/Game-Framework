// Copyright 2019. All Rights Reserved.
namespace GameFramework.Deterministic
{
	public struct Vector2
	{
		public static readonly Vector2 Zero = new Vector2(0, 0);
		public static readonly Vector2 One = new Vector2(1, 1);
		public static readonly Vector2 Up = new Vector2(0, 1);
		public static readonly Vector2 Right = new Vector2(1, 0);

		public Number X;
		public Number Y;

		public Number SqrMagnitude
		{
			get { return (X * X) + (Y * Y); }
		}

		public Number Magnitude
		{
			get { return Math.Sqrt(SqrMagnitude); }
		}

		public Vector2 Normalized
		{
			get
			{
				Vector2 value = this;
				value.Normalize();
				return value;
			}
		}

		public Vector2(Number X, Number Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public void Normalize()
		{
			Number value = Magnitude;

			if (Math.IsZero(value))
				return;

			X /= value;
			Y /= value;
		}

		public Number Dot(Vector2 Other)
		{
			return (X * Other.X) + (Y * Other.Y);
		}

		public Number Cross(Vector2 Other)
		{
			return (X * Other.Y) - (Y * Other.X);
		}

		public static Vector2 operator -(Vector2 Other)
		{
			return new Vector2(-Other.X, -Other.Y);
		}

		public static Vector2 operator *(Vector2 Left, Number Right)
		{
			return new Vector2(Left.X * Right, Left.Y * Right);
		}

		public static Vector2 operator /(Vector2 Left, Number Right)
		{
			return new Vector2(Left.X / Right, Left.Y / Right);
		}

		public static Vector2 operator +(Vector2 Left, Vector2 Right)
		{
			return new Vector2(Left.X + Right.X, Left.Y + Right.Y);
		}

		public static Vector2 operator -(Vector2 Left, Vector2 Right)
		{
			return new Vector2(Left.X - Right.X, Left.Y - Right.Y);
		}

		public static bool operator ==(Vector2 Left, Vector2 Right)
		{
			return (Left.X == Right.X && Left.Y == Right.Y);
		}

		public static bool operator !=(Vector2 Left, Vector2 Right)
		{
			return !(Left == Right);
		}

		public static bool operator >(Vector2 Left, Vector2 Right)
		{
			return (Left.X > Right.X && Left.Y > Right.Y);
		}

		public static bool operator >=(Vector2 Left, Vector2 Right)
		{
			return (Left.X >= Right.X && Left.Y >= Right.Y);
		}

		public static bool operator <(Vector2 Left, Vector2 Right)
		{
			return (Left.X < Right.X && Left.Y < Right.Y);
		}

		public static bool operator <=(Vector2 Left, Vector2 Right)
		{
			return (Left.X <= Right.X && Left.Y <= Right.Y);
		}

		public override string ToString()
		{
			return "(" + X + ", " + Y + ")";
		}
	}
}