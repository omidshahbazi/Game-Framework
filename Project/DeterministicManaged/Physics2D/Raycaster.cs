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

		private static readonly Func<Body, Vector2, Vector2, DispatchResult>[] Dispatchers = new Func<Body, Vector2, Vector2, DispatchResult>[] { DispatchCircleShape, DispatchPolygonShape };

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

				DispatchResult result = DispatchShape(body, Info.Origin, endPoint);
				if (!result.Hit)
					continue;

				Result.Body = body;
				Result.Point = result.Point;

				return true;
			}

			return false;
		}

		private static DispatchResult DispatchShape(Body Body, Vector2 LineStartPoint, Vector2 LineEndPoint)
		{
			return Dispatchers[(int)Body.Shape.GetType()](Body, LineStartPoint, LineEndPoint);
		}

		//private static DispatchResult DispatchCircleShape(Body Body, Vector2 LineStartPoint, Vector2 LineEndPoint)
		//{
		//	CircleShape shape = (CircleShape)Body.Shape;

		//	Vector2 lineDiff = LineEndPoint - LineStartPoint;
		//	Number lineLenSqr = lineDiff.SqrMagnitude;
		//	if (lineLenSqr == 0)
		//		return false;

		//	Vector2 centerDiff = Body.Position - LineStartPoint;
		//	Number centerLenSqr = centerDiff.SqrMagnitude - (shape.Radius * shape.Radius);

		//	Number lineCenterDot = lineDiff.Dot(centerDiff);
		//	Number pBy2 = lineCenterDot / lineLenSqr;

		//	Number q = centerLenSqr / lineLenSqr;

		//	Number disc = pBy2 * pBy2 - q;
		//	if (disc < 0)
		//		return false;

		//	Number tmpSqrt = Math.Sqrt(disc);
		//	Number abScalingFactor1 = -pBy2 + tmpSqrt;
		//	Number abScalingFactor2 = -pBy2 - tmpSqrt;

		//	Vector2 p1 = (LineStartPoint - lineDiff) * abScalingFactor1;
		//	if (disc == 0)
		//	{
		//		//return Collections.singletonList(p1);

		//		return true;
		//	}

		//	Vector2 p2 = (LineStartPoint - lineDiff) * abScalingFactor2;

		//	//return Arrays.asList(p1, p2);

		//	return true;
		//}

		private static DispatchResult DispatchCircleShape(Body Body, Vector2 LineStartPoint, Vector2 LineEndPoint)
		{
			CircleShape shape = (CircleShape)Body.Shape;

			Vector2 diff = LineEndPoint - LineStartPoint;

			Vector2 centerDiff = -(Body.Position - LineStartPoint);

			Number A = diff.SqrMagnitude;
			Number B = 2 * ((diff.X * centerDiff.X) + (diff.Y * centerDiff.Y));
			Number C = (centerDiff.X * centerDiff.X) + (centerDiff.Y * centerDiff.Y) - (shape.Radius * shape.Radius);

			Number det = B * B - 4 * A * C;

			if ((A <= Math.Epsilon) || (det < 0))
				return new DispatchResult() { Hit = false };

			Number t = 0;

			if (det == 0)
			{
				// One solution.
				t = -B / (2 * A);

				return new DispatchResult() { Hit = true, Distance = 0, Point = LineStartPoint + (diff * t) };
			}

			// Two solutions.
			t = (-B + Math.Sqrt(det)) / (2 * A);

			t = (float)((-B - Math.Sqrt(det)) / (2 * A));

			return new DispatchResult() { Hit = true, Distance = 0, Point = LineStartPoint + (diff * t) };
		}

		private static DispatchResult DispatchPolygonShape(Body Body, Vector2 LineStartPoint, Vector2 LineEndPoint)
		{
			return new DispatchResult() { Hit = false };
		}
	}
}