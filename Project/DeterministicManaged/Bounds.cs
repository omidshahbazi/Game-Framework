// Copyright 2019. All Rights Reserved.
using System.Runtime.CompilerServices;

namespace GameFramework.Deterministic
{
	public struct Bounds
	{
		public Vector2 Position;
		public Vector2 Size;

		[CompilerGenerated]
		public Vector2 Center
		{
			get { return Position + (Size / 2); }
		}

		[CompilerGenerated]
		public Vector2 Min
		{
			get { return Position; }
		}

		[CompilerGenerated]
		public Vector2 Max
		{
			get { return Position + Size; }
		}

		public Bounds(Number X, Number Y, Number Width, Number Height)
		{
			Position.X = X;
			Position.Y = Y;
			Size.X = Width;
			Size.Y = Height;
		}

		public Bounds(Vector2 Position, Vector2 Size)
		{
			this.Position = Position;
			this.Size = Size;
		}

		public bool Contains(Vector2 Point)
		{
			return Physics.BoundsContains(Min, Max, Point);
		}

		public void Expand(Vector2 Amount)
		{
			Amount /= 2;
			Position -= Amount;
			Size += Amount;
		}

		public bool Intersects(Bounds Bounds)
		{
			return Physics.BoundsIntersect(Min, Max, Bounds.Min, Bounds.Max);
		}

		public override string ToString()
		{
			return "[(" + Position + "), (" + Size + ")]";
		}
	}
}