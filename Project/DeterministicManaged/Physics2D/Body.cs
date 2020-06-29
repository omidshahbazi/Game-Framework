// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics2D
{
	public class Body : IVisitee
	{
		public Vector2 Position;
		public Matrix2 Orientation;
		public Number Mass;
		public Number Inertia;
		public Number Restitution;
		public Number DynamicFriction;
		public Number StaticFriction;

		public Vector2 Force;
		public Vector2 Velocity;
		public Number AngularVelocity;
		public Number Torque;

		public Shape Shape;

		public void Visit(IVisitor Visitor)
		{
			Visitor.VisitVector2(Position);
			Visitor.VisitMatrix2(Orientation);
			Visitor.VisitNumber(Mass);
			Visitor.VisitNumber(Inertia);
			Visitor.VisitNumber(Restitution);
			Visitor.VisitNumber(DynamicFriction);
			Visitor.VisitNumber(StaticFriction);

			Visitor.VisitVector2(Force);
			Visitor.VisitVector2(Velocity);
			Visitor.VisitNumber(AngularVelocity);
			Visitor.VisitNumber(Torque);

			Shape.Visit(Visitor);
		}
	}
}