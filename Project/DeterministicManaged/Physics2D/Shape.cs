// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics2D
{
	public abstract class Shape : IVisitee
	{
		public enum Types
		{
			Circle = 0,
			Polygon
		}

		public abstract Types GetType();

		public virtual void Visit(IVisitor Visitor)
		{
			Visitor.VisitInt32((int)GetType());
		}
	}
}