// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics
{
	public class PolygonShape : Shape
	{
		public Vector3[] Vertices;
		public Vector3[] Normals;

		public override void Visit(IVisitor Visitor)
		{
			base.Visit(Visitor);

			Visitor.VisitArray(Vertices);
			Visitor.VisitArray(Normals);
		}
	}
}