// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics
{
	public class Body : IVisitee
	{
		public Vector3 Position;
		public Matrix3 Orientation;
		public Number Mass;
		public Number Inertia;
		public Number Restitution;
		public Number DynamicFriction;
		public Number StaticFriction;

		public Vector3 Force;
		public Vector3 Velocity;
		public Vector3 AngularVelocity;
		public Number Torque;

		public Shape Shape;

		public void Visit(IVisitor Visitor)
		{
			Visitor.VisitVector3(Position);
			Visitor.VisitMatrix3(Orientation);
			Visitor.VisitNumber(Mass);
			Visitor.VisitNumber(Inertia);
			Visitor.VisitNumber(Restitution);
			Visitor.VisitNumber(DynamicFriction);
			Visitor.VisitNumber(StaticFriction);

			Visitor.VisitVector3(Force);
			Visitor.VisitVector3(Velocity);
			Visitor.VisitVector3(AngularVelocity);
			Visitor.VisitNumber(Torque);

			Shape.Visit(Visitor);
		}
	}
}