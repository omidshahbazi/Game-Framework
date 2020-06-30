using GameFramework.Deterministic;
using GameFramework.Deterministic.Physics2D;
using GameFramework.GDIRenderer;
using System.Drawing;

namespace DeterministicTest
{
	static class Drawer
	{
		private const float SCALE = 1;

		public static void Draw(IDevice Device, Scene Scene)
		{
			for (int i = 0; i < Scene.Bodies.Length; ++i)
				DrawBody(Device, Scene.Bodies[i], Pens.Black);
		}

		public static void Draw(IDevice Device, ContactList Contacts)
		{
			for (int i = 0; i < Contacts.Count; ++i)
				DrawContact(Device, Contacts[i], Pens.Red);
		}

		public static void Draw(IDevice Device, Raycaster.Info Info)
		{
			DrawVector(Device, Pens.Blue, Info.Origin, Info.Direction * Info.Distance);
		}

		public static void Draw(IDevice Device, Raycaster.Result Result)
		{
			DrawCircle(Device, Pens.Red, Result.Point, 2);
		}

		private static void DrawContact(IDevice Device, Contact Contact, Pen Pen)
		{
			for (int i = 0; i < Contact.Points.Length; ++i)
				DrawCircle(Device, Pen, Contact.Points[i], 2);
		}

		private static void DrawBody(IDevice Device, Body Body, Pen Pen)
		{
			DrawVector(Device, Pens.Red, Body.Position, Body.Velocity.Normalized * SCALE);

			DrawShape(Device, Body, Pen);
		}

		private static void DrawShape(IDevice Device, Body Body, Pen Pen)
		{
			if (Body.Shape is CircleShape)
				DrawCircleShape(Device, Body, (CircleShape)Body.Shape, Body.Position, Pen);
			else if (Body.Shape is PolygonShape)
				DrawPolygonShape(Device, Body, (PolygonShape)Body.Shape, Body.Position, Pen);
		}

		private static void DrawCircleShape(IDevice Device, Body Body, CircleShape Shape, Vector2 CenterPosition, Pen Pen)
		{
			Number radius = Shape.Radius * SCALE;

			DrawCircle(Device, Pen, CenterPosition - (Vector2.One * radius), radius);

			DrawVector(Device, Pen, CenterPosition, (Body.Orientation * Vector2.Right) * radius);
		}

		private static void DrawPolygonShape(IDevice Device, Body Body, PolygonShape Shape, Vector2 CenterPosition, Pen Pen)
		{
			if (Shape.Vertices.Length < 2)
				return;

			for (int i = 1; i < Shape.Vertices.Length; ++i)
			{
				Vector2 pointA = CenterPosition + (Body.Orientation * Shape.Vertices[i - 1] * SCALE);
				Vector2 pointB = CenterPosition + (Body.Orientation * Shape.Vertices[i] * SCALE);

				DrawLine(Device, Pen, pointA, pointB);

				DrawVector(Device, Pen, pointA, Shape.Normals[i - 1] * SCALE);
				DrawVector(Device, Pen, pointB, Shape.Normals[i] * SCALE);
			}

			DrawLine(Device, Pen, CenterPosition + (Body.Orientation * Shape.Vertices[0] * SCALE), CenterPosition + (Body.Orientation * Shape.Vertices[Shape.Vertices.Length - 1] * SCALE));
		}

		private static void DrawVector(IDevice Device, Pen Pen, Vector2 From, Vector2 Vector)
		{
			DrawLine(Device, Pen, From, From + Vector);
		}

		private static void DrawCircle(IDevice Device, Pen Pen, Vector2 Origin, Number Radius)
		{
			Device.DrawCircle(Origin.X, Origin.Y, Radius, Pen);
		}

		private static void DrawLine(IDevice Device, Pen Pen, Vector2 From, Vector2 To)
		{
			Device.DrawLine(From.X, From.Y, To.X, To.Y, Pen);
		}
	}
}
