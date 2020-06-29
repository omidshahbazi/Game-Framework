// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics2D
{
	public class PolygonShape : Shape
	{
		public Vector2[] Vertices;
		public Vector2[] Normals;

		public override Types GetType()
		{
			return Types.Polygon;
		}

		public override void Visit(IVisitor Visitor)
		{
			base.Visit(Visitor);

			Visitor.VisitArray(Vertices);
			Visitor.VisitArray(Normals);
		}
	}
}