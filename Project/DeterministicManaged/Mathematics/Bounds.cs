// Copyright 2019. All Rights Reserved.
using System.Runtime.CompilerServices;

namespace GameFramework.Deterministic.Mathematics
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
			return (Point >= Min && Point <= Max);
		}

		public void Expand(Vector2 Amount)
		{
			Amount /= 2;
			Position -= Amount;
			Size += Amount;
		}

		public bool Intersects(Bounds Bounds)
		{
			return
				Contains(Bounds.Min) ||
				Contains(Bounds.Max) ||
				Contains(Bounds.Position + new Vector2(Size.X, 0)) ||
				Contains(Bounds.Position + new Vector2(0, Size.Y));
		}

		public override string ToString()
		{
			return "[(" + Position + "), (" + Size + ")]";
		}
	}
}