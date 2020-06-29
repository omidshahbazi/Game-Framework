// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics2D
{
	public class CircleShape : Shape
	{
		public Number Radius;

		public override Types GetType()
		{
			return Types.Circle;
		}

		public override void Visit(IVisitor Visitor)
		{
			base.Visit(Visitor);

			Visitor.VisitNumber(Radius);
		}
	}
}