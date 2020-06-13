// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics
{
	public class Body : IVisitee
	{
		public Vector3 Position;
		public Matrix3 Rotation;
		public Vector3 Velocity;
		public Vector3 Orientation;
		public Number Mass;

		public Shape Shape;

		public void Visit(IVisitor Visitor)
		{
			Visitor.VisitVector3(Position);
			Visitor.VisitVector3(Velocity);
			Visitor.VisitVector3(Orientation);
			Visitor.VisitNumber(Mass);

			Shape.Visit(Visitor);
		}
	}
}