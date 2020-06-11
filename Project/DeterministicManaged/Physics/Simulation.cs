// Copyright 2019. All Rights Reserved.

using System;

namespace GameFramework.Deterministic.Physics
{
	public static class Simulation
	{
		public class Config
		{
			public Number StepTime;
			public int CCDStepCount = 10;
		}

		public static void Simulate(Scene Scene, Config Config, ContactList Contacts)
		{
			for (int i = 0; i < Scene.Bodies.Length; ++i)
			{
				Body a = Scene.Bodies[i];

				for (int j = i + 1; j < Scene.Bodies.Length; ++j)
				{
					Body b = Scene.Bodies[j];

					if (a.Mass == 0 && b.Mass == 0)
						continue;

					Contact contact = new Contact() { BodyA = a, BodyB = b };

					DispatchContact(contact);

					if (contact.Points != null && contact.Points.Length != 0)
						Contacts.Add(contact);
				}
			}

			for (int i = 0; i < Scene.Bodies.Length; ++i)
				IntegrateForces(Scene.Bodies[i], Config);

			for (int i = 0; i < Contacts.Count; ++i)
				InitializeContact(Contacts[i]);

			for (int i = 0; i < Config.CCDStepCount; ++i)
				for (int j = 0; j < Contacts.Count; ++j)
					SolveContact(Contacts[j]);

			for (int i = 0; i < Scene.Bodies.Length; ++i)
				IntegrateVelocity(Scene.Bodies[i]);

			for (int i = 0; i < Contacts.Count; ++i)
				CorrectPosition(Contacts[i]);

			for (int i = 0; i < Scene.Bodies.Length; ++i)
			{
				Body body = Scene.Bodies[i];

				//TODO: remove any force and torque
			}
		}

		public static void DispatchContact(Contact Contact)
		{
			bool aIsCircle = (Contact.BodyA.Shape is CircleShape);
			bool bIsCircle = (Contact.BodyB.Shape is CircleShape);

			if (aIsCircle)
			{
				if (bIsCircle)
					DispatchCircleToCircle(Contact);
				else
					DispatchCircleToPolygon(Contact);
			}
			else
			{
				if (bIsCircle)
					DispatchPolygonToCircle(Contact);
				else
					DispatchPolygonToPolygon(Contact);
			}
		}

		public static void InitializeContact(Contact Contact)
		{

		}

		public static void SolveContact(Contact Contact)
		{

		}

		public static void IntegrateForces(Body Body, Config Config)
		{

		}

		private static void IntegrateVelocity(Body Body)
		{
		}

		private static void CorrectPosition(Contact Contact)
		{
		}

		public static void DispatchCircleToCircle(Contact Contact)
		{
			CircleShape a = (CircleShape)Contact.BodyA.Shape;
			CircleShape b = (CircleShape)Contact.BodyB.Shape;

			// Calculate translational vector, which is normal
			Vector3 normal = Contact.BodyB.Position - Contact.BodyA.Position;

			Number dist_sqr = normal.SqrMagnitude;
			Number radius = a.Radius + b.Radius;

			// Not in contact
			if (dist_sqr >= radius * radius)
			{
				Contact.Points = null;

				return;
			}

			Number distance = Math.Sqrt(dist_sqr);

			Contact.Points = new Vector3[1];

			if (distance == 0.0f)
			{
				//Contact.penetration = A->radius;
				Contact.Normal = Vector3.RIGHT;
				Contact.Points[0] = Contact.BodyA.Position;

				return;
			}

			//Contact.penetration = radius - distance;
			Contact.Normal = normal / distance; // Faster than using Normalized since we already performed sqrt
			
			Contact.Points[0] = Contact.Normal * a.Radius + Contact.BodyA.Position;
		}

		public static void DispatchCircleToPolygon(Contact Contact)
		{
		}

		public static void DispatchPolygonToPolygon(Contact Contact)
		{
		}

		public static void DispatchPolygonToCircle(Contact Contact)
		{
		}
	}
}