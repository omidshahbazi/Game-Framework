// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.Deterministic.Physics2D
{
	public static class Raycaster
	{
		public class Info
		{
			public Scene Scene;
			public Vector2 Origin;
			public Vector2 Direction;
			public Number Distance;
		}

		public struct Result
		{
			public Body Body;
			public Vector2 Point;
		}

		private struct DispatchResult
		{
			public bool Hit;
			public Number Distance;
			public Vector2 Point;
		}

		private static readonly Func<Body, Info, Vector2, DispatchResult>[] Dispatchers = new Func<Body, Info, Vector2, DispatchResult>[] { DispatchCircleShape, DispatchPolygonShape };

		public static bool Raycast(Body[] Bodies, Vector2 Origin, Vector2 Direction, Number Distance, ref Result Result)
		{
			return Raycast(new Scene() { Bodies = Bodies }, Origin, Direction, Distance, ref Result);
		}

		public static bool Raycast(Scene Scene, Vector2 Origin, Vector2 Direction, Number Distance, ref Result Result)
		{
			return Raycast(new Info() { Scene = Scene, Origin = Origin, Direction = Direction, Distance = Distance }, ref Result);
		}

		public static bool Raycast(Info Info, ref Result Result)
		{
			Vector2 endPoint = Info.Origin + (Info.Direction * Info.Distance);

			DispatchResult[] results = new DispatchResult[Info.Scene.Bodies.Length];

			for (int i = 0; i < Info.Scene.Bodies.Length; ++i)
			{
				Body body = Info.Scene.Bodies[i];

				results[i] = DispatchShape(body, Info, endPoint);
			}

			int index = -1;
			DispatchResult minResult = FindMinimumDistanced(results, ref index);

			if (!minResult.Hit)
				return false;

			Result.Body = Info.Scene.Bodies[index];
			Result.Point = minResult.Point;

			return true;
		}

		private static DispatchResult DispatchShape(Body Body, Info Info, Vector2 LineEndPoint)
		{
			return Dispatchers[(int)Body.Shape.GetType()](Body, Info, LineEndPoint);
		}

		private static DispatchResult DispatchCircleShape(Body Body, Info Info, Vector2 LineEndPoint)
		{
			CircleShape shape = (CircleShape)Body.Shape;

			Vector2 lineDiff = LineEndPoint - Info.Origin;
			Number lineDiffSqr = lineDiff.SqrMagnitude;

			Vector2 centerDiff = Body.Position - Info.Origin;
			Number centerDiffSqr = centerDiff.SqrMagnitude;

			Number radiusSqr = shape.Radius * shape.Radius;

			if ((Info.Origin + (Info.Direction * Math.Min(Info.Distance, centerDiff.Magnitude)) - Body.Position).SqrMagnitude > radiusSqr)
				return new DispatchResult() { Hit = false };

			centerDiff *= -1;

			Number B = 2 * ((lineDiff.X * centerDiff.X) + (lineDiff.Y * centerDiff.Y));
			Number C = centerDiffSqr - radiusSqr;

			Number det = B * B - 4 * lineDiffSqr * C;

			if ((lineDiffSqr <= Math.Epsilon) || (det < 0))
				return new DispatchResult() { Hit = false };

			Number distance = 0;

			if (det == 0) // One solution
				distance = -B / (2 * lineDiffSqr);
			else // Two solutions
			{
				// Farest point
				//distance = (-B + Math.Sqrt(det)) / (2 * A);

				// Nearest point
				distance = (float)((-B - Math.Sqrt(det)) / (2 * lineDiffSqr));
			}

			return new DispatchResult() { Hit = true, Distance = distance, Point = Info.Origin + (lineDiff * distance) };
		}

		private static DispatchResult DispatchPolygonShape(Body Body, Info Info, Vector2 LineEndPoint)
		{
			PolygonShape shape = (PolygonShape)Body.Shape;

			DispatchResult[] results = new DispatchResult[shape.Vertices.Length];

			for (int i = 0; i < shape.Vertices.Length; ++i)
			{
				Vector2 vertex1 = Body.Position + (Body.Orientation * shape.Vertices[i == 0 ? shape.Vertices.Length - 1 : i - 1]);
				Vector2 vertex2 = Body.Position + (Body.Orientation * shape.Vertices[i]);

				results[i] = TestLineIntersectsLine(vertex1, vertex2, Info.Origin, LineEndPoint);
			}

			int index = -1;
			return FindMinimumDistanced(results, ref index);
		}

		private static DispatchResult TestLineIntersectsLine(Vector2 Line1StartPoint, Vector2 Line1EndPoint, Vector2 Line2StartPoint, Vector2 Line2EndPoint)
		{
			Vector2 diffLine1 = Line1EndPoint - Line1StartPoint;
			Vector2 diffLine2 = Line2EndPoint - Line2StartPoint;

			Number x = (-diffLine2.X * diffLine1.Y) + (diffLine2.Y * diffLine1.X);
			if (x == 0)
				return new DispatchResult() { Hit = false };

			Vector2 diffStartPoint = Line1StartPoint - Line2StartPoint;

			Number s = ((-diffLine1.Y * diffStartPoint.X) + (diffLine1.X * diffStartPoint.Y)) / x;
			Number t = ((diffLine2.X * diffStartPoint.Y) - (diffLine2.Y * diffStartPoint.X)) / x;

			if (0 <= s && s <= 1 &&
				0 <= t && t <= 1)
			{
				Vector2 vec = diffLine1 * t;

				return new DispatchResult() { Hit = true, Distance = vec.Magnitude, Point = Line1StartPoint + vec };
			}

			return new DispatchResult() { Hit = false };
		}

		private static DispatchResult FindMinimumDistanced(DispatchResult[] Results, ref int Index)
		{
			DispatchResult minResult = new DispatchResult() { Hit = false, Distance = Number.MaxValue };

			for (int i = 0; i < Results.Length; ++i)
			{
				if (!Results[i].Hit)
					continue;

				if (minResult.Distance < Results[i].Distance)
					continue;

				minResult = Results[i];
				Index = i;
			}

			return minResult;
		}
	}
}