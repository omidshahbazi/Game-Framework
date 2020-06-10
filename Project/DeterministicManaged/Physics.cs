// Copyright 2019. All Rights Reserved.
namespace GameFramework.Deterministic
{
	public static class Physics
	{
		public static Number CompareDistance(Vector3 A, Vector3 B, Number Distance)
		{
			A.Y = B.Y;
			Number num = (A - B).SqrMagnitude;
			Number sqDis = Distance * Distance;

			return Math.Sign(num - sqDis);
		}

		public static bool LinesIntersectsLine(Vector3 StartPoint1, Vector3 EndPoint1, Vector3 StartPoint2, Vector3 EndPoint2)
		{
			Vector3 v1Diff = EndPoint1 - StartPoint1;
			Vector3 v2Diff = EndPoint2 - StartPoint2;

			float denom = (v2Diff.Z * v1Diff.X) - (v2Diff.X * v1Diff.Z);

			float numerator = (v2Diff.X * (StartPoint1.Z - StartPoint2.Z)) - (v2Diff.Z * (StartPoint1.X - StartPoint2.X));
			float numerator2 = (v1Diff.X * (StartPoint1.Z - StartPoint2.Z)) - (v1Diff.Z * (StartPoint1.X - StartPoint2.X));

			if (denom == 0.0f)
			{
				if (numerator == 0.0f && numerator2 == 0.0f)
					return false; //COINCIDENT;

				return false; // PARALLEL;
			}

			float ua = numerator / denom;
			float ub = numerator2 / denom;

			return (ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f);
		}

		public static bool BoundsIntersectsBounds(Vector3 Min1, Vector3 Max1, Vector3 Min2, Vector3 Max2)
		{
			Vector3 d1 = Min2 - Max1;
			Vector3 d2 = Min1 - Max2;

			if (d1.X > 0 || d1.Y > 0 || d1.Z > 0)
				return false;

			if (d2.X > 0 || d2.Y > 0 || d2.Z > 0)
				return false;

			return true;
		}

		public static bool LineIntersectsBounds( Vector3 StartPoint, Vector3 EndPoint, Vector3 Min, Vector3 Max)
		{
			Vector3 lowerLeft = Min;
			Vector3 upperRight = Max;
			Vector3 upperLeft = new Vector3(lowerLeft.X, 0, upperRight.Z);
			Vector3 lowerRight = new Vector3(upperRight.X, 0, lowerLeft.Z);

			if (StartPoint.X > lowerLeft.X && StartPoint.X < upperRight.X && StartPoint.Z < lowerLeft.Z && StartPoint.Z > upperRight.Z &&
				EndPoint.X > lowerLeft.X && EndPoint.X < upperRight.X && EndPoint.Z < lowerLeft.Z && EndPoint.Z > upperRight.Z)
			{
				return true;
			}

			if (LinesIntersectsLine(StartPoint, EndPoint, upperLeft, lowerLeft)) return true;
			if (LinesIntersectsLine(StartPoint, EndPoint, lowerLeft, lowerRight)) return true;
			if (LinesIntersectsLine(StartPoint, EndPoint, upperLeft, upperRight)) return true;
			if (LinesIntersectsLine(StartPoint, EndPoint, upperRight, lowerRight)) return true;

			return false;
		}

		public static bool BoundsContainsPoint(Vector3 Min, Vector3 Max, Vector3 Point)
		{
			return (Min <= Point && Point <= Max);
		}
	}
}