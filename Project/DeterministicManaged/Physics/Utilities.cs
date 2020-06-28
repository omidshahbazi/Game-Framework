// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Extensions;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameFramework.Deterministic.Physics
{
	public static class Utilities
	{
		public static Body AddBody(Scene Scene)
		{
			Body body = new Body();
			ArrayUtilities.Add(ref Scene.Bodies, body);

			body.Orientation = Matrix3.Identity;

			return body;
		}

		public static SphereShape CreateSphereShape(Number Radius)
		{
			return new SphereShape() { Radius = Radius };
		}

		public static PolygonShape CreatePolygonShape(Vector3[] Vertices)
		{
			//TODO: handle Z axis

			PolygonShape shape = new PolygonShape();

			Debug.Assert(Vertices.Length > 3, "Vertices count must be greater than 3");

			// Find the right most point on the hull
			uint rightMost = 0;
			Number highestXCoord = Vertices[0].X;
			for (uint i = 1; i < Vertices.Length; ++i)
			{
				Number x = Vertices[i].X;
				if (x > highestXCoord)
				{
					highestXCoord = x;
					rightMost = i;
				}

				// If matching x then take farthest negative y
				else if (x == highestXCoord)
					if (Vertices[i].Y < Vertices[rightMost].Y)
						rightMost = i;
			}

			List<uint> hull = new List<uint>();
			uint indexHull = rightMost;

			while (true)
			{
				hull.Add(indexHull);

				// Search for next index that wraps around the hull
				// by computing cross products to find the most counter-clockwise
				// vertex in the set, given the previos hull index
				uint nextHullIndex = 0;
				for (uint i = 1; i < Vertices.Length; ++i)
				{
					// Skip if same coordinate as we need three unique
					// points in the set to perform a cross product
					if (nextHullIndex == indexHull)
					{
						nextHullIndex = i;
						continue;
					}

					// Cross every set of three unique vertices
					// Record each counter clockwise third vertex and add
					// to the output hull
					// See : http://www.oocities.org/pcgpe/math2d.html
					Vector3 e1 = Vertices[nextHullIndex] - Vertices[indexHull];
					Vector3 e2 = Vertices[i] - Vertices[indexHull];

					//TODO: Fix for Z axis
					Number c = (e1 * e2).Z;

					if (c < 0.0f)
						nextHullIndex = i;

					// Cross product is zero then e vectors are on same line
					// therefor want to record vertex farthest along that line
					if (c == 0.0f && e2.SqrMagnitude > e1.SqrMagnitude)
						nextHullIndex = i;
				}

				indexHull = nextHullIndex;

				// Conclude algorithm upon wrap-around
				if (nextHullIndex == rightMost)
				{
					shape.Vertices = new Vector3[hull.Count];
					shape.Normals = new Vector3[hull.Count];
					break;
				}
			}

			// Copy vertices into shape's vertices
			for (int i = 0; i < shape.Vertices.Length; ++i)
				shape.Vertices[i] = Vertices[hull[i]];

			// Compute face normals
			for (uint i1 = 0; i1 < shape.Normals.Length; ++i1)
			{
				uint i2 = i1 + 1 < shape.Normals.Length ? i1 + 1 : 0;
				Vector3 face = shape.Vertices[i2] - shape.Vertices[i1];

				// Ensure no zero-length edges, because that's bad
				Debug.Assert(face.SqrMagnitude > Math.Epsilon * Math.Epsilon);

				// Calculate normal with 2D cross product between vector and scalar
				shape.Normals[i1] = new Vector3(face.Y, -face.X, 0);
				shape.Normals[i1].Normalize();
			}

			return shape;
		}

		public static PolygonShape CreateSquareShape(Vector2 Size, Vector2 Offset)
		{
			Vector2 halfSize = Size * 0.5F;
			Vector3 offset = new Vector3(Offset.X, Offset.Y, 0);

			return CreatePolygonShape(
				new Vector3[] {
					new Vector3(-halfSize.X, -halfSize.Y, 0) + offset,
					new Vector3(-halfSize.X, halfSize.Y, 0) + offset,
					new Vector3(halfSize.X, halfSize.Y, 0) + offset,
					new Vector3(halfSize.X, -halfSize.Y, 0) + offset });
		}

		public static PolygonShape CreateCubeShape(Vector3 Size, Vector3 Offset)
		{
			//TODO: This is not a cube
			//Vector3 halfSize = Size * 0.5F;

			//return CreatePolygonShape(
			//	new Vector3[] {
			//		-halfSize + Offset,
			//		new Vector3(-halfSize.X, halfSize.Y, 0) + Offset,
			//		new Vector3(halfSize.X, halfSize.Y, 0) + Offset,
			//		new Vector3(halfSize.X, -halfSize.Y, 0) + Offset });

			return null;
		}

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

		public static bool LineIntersectsBounds(Vector3 StartPoint, Vector3 EndPoint, Vector3 Min, Vector3 Max)
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