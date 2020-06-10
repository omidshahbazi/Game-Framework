// Copyright 2019. All Rights Reserved.
using System.Runtime.CompilerServices;

namespace GameFramework.Deterministic
{
	public struct Bounds
	{
		public Vector3 Position;
		public Vector3 Size;

		[CompilerGenerated]
		public Vector3 Center
		{
			get { return Position + (Size / 2); }
		}

		[CompilerGenerated]
		public Vector3 Min
		{
			get { return Position; }
		}

		[CompilerGenerated]
		public Vector3 Max
		{
			get { return Position + Size; }
		}

		public Bounds(Number X, Number Y, Number Z, Number Width, Number Height, Number Depth)
		{
			Position.X = X;
			Position.Y = Y;
			Position.Z = Z;
			Size.X = Width;
			Size.Y = Height;
			Size.Z = Depth;
		}

		public Bounds(Vector3 Position, Vector3 Size)
		{
			this.Position = Position;
			this.Size = Size;
		}

		public bool Contains(Vector3 Point)
		{
			return Physics.BoundsContainsPoint(Min, Max, Point);
		}

		public void Expand(Vector3 Amount)
		{
			Amount /= 2;
			Position -= Amount;
			Size += Amount;
		}

		public bool Intersects(Bounds Bounds)
		{
			return Physics.BoundsIntersectsBounds(Min, Max, Bounds.Min, Bounds.Max);
		}

		public override string ToString()
		{
			return "[(" + Position + "), (" + Size + ")]";
		}
	}
}