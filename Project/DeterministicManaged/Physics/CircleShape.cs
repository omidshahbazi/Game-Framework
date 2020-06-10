// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics
{
	public abstract class CircleShape : Shape
	{
		public Number Radius;

		public override void Visit(IVisitor Visitor)
		{
			base.Visit(Visitor);

			Visitor.VisitNumber(Radius);
		}
	}
}