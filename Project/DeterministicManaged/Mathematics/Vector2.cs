﻿// Copyright 2019. All Rights Reserved.
namespace GameFramework.Deterministic.Mathematics
{
	public struct Vector2
	{
		public static readonly Vector2 ZERO = new Vector2(0, 0);
		public static readonly Vector2 ONE = new Vector2(1, 1);
		public static readonly Vector2 UP = new Vector2(0, 1);
		public static readonly Vector2 RIGHT = new Vector2(1, 0);

		public Number X;
		public Number Y;

		public Vector2(Number X, Number Y)
		{
			this.X = X;
			this.Y = Y;
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

		public static bool operator <(Vector2 Left, Vector2 Right)
		{
			return (Left.X < Right.X && Left.Y < Right.Y);
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector2)
				return ((Vector2)obj) == this;
			else
				return false;
		}

		public override string ToString()
		{
			return "V2[" + X + ", " + Y + "]";
		}
	}
}