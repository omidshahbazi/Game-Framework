﻿// Copyright 2019. All Rights Reserved.
namespace GameFramework.Deterministic
{
	public struct Vector3
	{
		public static readonly Vector3 ZERO = new Vector3(0, 0, 0);
		public static readonly Vector3 ONE = new Vector3(1, 1, 1);
		public static readonly Vector3 UP = new Vector3(0, 1, 0);
		public static readonly Vector3 RIGHT = new Vector3(1, 0, 0);
		public static readonly Vector3 FORWARD = new Vector3(0, 0, 1);

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
			if (X == 0 && Y == 0)
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

		public static Vector3 operator *(Vector3 Left, Number Right)
		{
			return new Vector3(Left.X * Right, Left.Y * Right, Left.Z * Right);
		}

		public static Vector3 operator /(Vector3 Left, Number Right)
		{
			return new Vector3(Left.X / Right, Left.Y / Right, Left.Z / Right);
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
			return "V2[" + X + ", " + Y + ", " + Z + "]";
		}
	}
}