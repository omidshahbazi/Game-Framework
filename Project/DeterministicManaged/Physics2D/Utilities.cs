// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Extensions;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameFramework.Deterministic.Physics2D
{
	public static class Utilities
	{
		public static Body AddBody(Scene Scene)
		{
			Body body = new Body();
			ArrayUtilities.Add(ref Scene.Bodies, body);

			body.Orientation = Matrix2.Identity;

			return body;
		}

		public static CircleShape CreateCircleShape(Number Radius)
		{
			return new CircleShape() { Radius = Radius };
		}

		public static PolygonShape CreatePolygonShape(Vector2[] Vertices)
		{
			PolygonShape shape = new PolygonShape();

			Utilities.SetPolygonVertices(shape, Vertices);

			return shape;
		}

		public static PolygonShape CreateSquareShape(Vector2 Size, Vector2 Offset)
		{
			Vector2 halfSize = Size * 0.5F;
			Vector2 offset = new Vector2(Offset.X, Offset.Y);

			return CreatePolygonShape(
				new Vector2[] {
					new Vector2(-halfSize.X, -halfSize.Y) + offset,
					new Vector2(-halfSize.X, halfSize.Y) + offset,
					new Vector2(halfSize.X, halfSize.Y) + offset,
					new Vector2(halfSize.X, -halfSize.Y) + offset });
		}

		public static void SetPolygonVertices(PolygonShape Shape, Vector2[] Vertices)
		{
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
					Vector2 e1 = Vertices[nextHullIndex] - Vertices[indexHull];
					Vector2 e2 = Vertices[i] - Vertices[indexHull];

					Number c = e1.Cross(e2);

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
					Shape.Vertices = new Vector2[hull.Count];
					Shape.Normals = new Vector2[hull.Count];
					break;
				}
			}

			// Copy vertices into shape's vertices
			for (int i = 0; i < Shape.Vertices.Length; ++i)
				Shape.Vertices[i] = Vertices[hull[i]];

			// Compute face normals
			for (uint i1 = 0; i1 < Shape.Normals.Length; ++i1)
			{
				uint i2 = i1 + 1 < Shape.Normals.Length ? i1 + 1 : 0;
				Vector2 face = Shape.Vertices[i2] - Shape.Vertices[i1];

				// Ensure no zero-length edges, because that's bad
				Debug.Assert(face.SqrMagnitude > Math.Epsilon * Math.Epsilon);

				// Calculate normal with 2D cross product between vector and scalar
				Shape.Normals[i1] = new Vector2(face.Y, -face.X);
				Shape.Normals[i1].Normalize();
			}
		}
	}
}