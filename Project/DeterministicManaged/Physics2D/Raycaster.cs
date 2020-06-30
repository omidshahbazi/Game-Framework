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

		private static readonly Func<Body, Vector2, Vector2, Vector2, DispatchResult>[] Dispatchers = new Func<Body, Vector2, Vector2, Vector2, DispatchResult>[] { DispatchCircleShape, DispatchPolygonShape };

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

			for (int i = 0; i < Info.Scene.Bodies.Length; ++i)
			{
				Body body = Info.Scene.Bodies[i];

				DispatchResult result = DispatchShape(body, Info.Origin, endPoint, Info.Direction);
				if (!result.Hit)
					continue;

				Result.Body = body;
				Result.Point = result.Point;

				return true;
			}

			return false;
		}

		private static DispatchResult DispatchShape(Body Body, Vector2 LineStartPoint, Vector2 LineEndPoint, Vector2 Direction)
		{
			return Dispatchers[(int)Body.Shape.GetType()](Body, LineStartPoint, LineEndPoint, Direction);
		}

		private static DispatchResult DispatchCircleShape(Body Body, Vector2 LineStartPoint, Vector2 LineEndPoint, Vector2 Direction)
		{
			CircleShape shape = (CircleShape)Body.Shape;

			Vector2 lineDiff = LineEndPoint - LineStartPoint;
			Number lineDiffSqr = lineDiff.SqrMagnitude;

			Vector2 centerDiff = Body.Position - LineStartPoint;
			Number centerDiffSqr = centerDiff.SqrMagnitude;

			Number radiusSqr = shape.Radius * shape.Radius;

			if (((LineStartPoint + (Direction * centerDiff.Magnitude)) - Body.Position).SqrMagnitude > radiusSqr)
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

			return new DispatchResult() { Hit = true, Distance = distance, Point = LineStartPoint + (lineDiff * distance) };
		}

		private static DispatchResult DispatchPolygonShape(Body Body, Vector2 LineStartPoint, Vector2 LineEndPoint, Vector2 Direction)
		{
			return new DispatchResult() { Hit = false };
		}
	}
}