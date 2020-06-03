// Copyright 2019. All Rights Reserved.
namespace GameFramework.Deterministic
{
	public static class Physics
	{
		public static Vector2 CalculateGravityForce(Vector2 Gravity, Number Mass)
		{
			return Gravity * Mass;
		}

		public static Vector2 CalculateAcceleration(Vector2 Force, Number Mass)
		{
			return Force / Mass;
		}

		public static Vector2 CalculateVelocity(Vector2 Acceleration, Number DeltaTime)
		{
			return Acceleration * DeltaTime;
		}

		public static Vector2 CalculateVelocity(Vector2 Force, Number Mass, Number DeltaTime)
		{
			return CalculateVelocity(CalculateAcceleration(Force, Mass), DeltaTime);
		}

		public static Vector2 CalculateMovement(Vector2 Velocity, Number DeltaTime)
		{
			return Velocity * DeltaTime;
		}

		public static Vector2 CalculateMovement(Vector2 Force, Number Mass, Number DeltaTime)
		{
			return CalculateMovement(CalculateVelocity(Force, Mass, DeltaTime), DeltaTime);
		}

		public static bool LinesIntersect(Vector2 StartPoint1, Vector2 EndPoint1, Vector2 StartPoint2, Vector2 EndPoint2, out Vector2 IntersectionPoint)
		{
			IntersectionPoint = Vector2.ZERO;

			Number a1 = EndPoint1.Y - StartPoint1.Y;
			Number b1 = StartPoint1.X - EndPoint1.X;
			Number c1 = a1 * (StartPoint1.X) + b1 * (StartPoint1.Y);

			Number a2 = EndPoint2.Y - StartPoint2.Y;
			Number b2 = StartPoint2.X - EndPoint2.X;
			Number c2 = a2 * (StartPoint2.X) + b2 * (StartPoint2.Y);

			Number determinant = a1 * b2 - a2 * b1;

			if (determinant == 0)
				return false;

			Number x = (b2 * c1 - b1 * c2) / determinant;
			Number y = (a1 * c2 - a2 * c1) / determinant;

			IntersectionPoint = new Vector2(x, y);

			return true;
		}

		public static bool BoundsContains(Vector2 Min, Vector2 Max, Vector2 Point)
		{
			return (Point >= Min && Point <= Max);
		}

		public static bool BoundsIntersect(Vector2 Min1, Vector2 Max1, Vector2 Min2, Vector2 Max2)
		{
			Vector2 d1 = Min2 - Max1;
			Vector2 d2 = Min1 - Max2;

			if (d1.X > 0 || d1.Y > 0)
				return false;

			if (d2.X > 0 || d2.Y > 0)
				return false;

			return true;
		}
	}
}