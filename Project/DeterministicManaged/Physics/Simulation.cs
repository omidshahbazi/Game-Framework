// Copyright 2019. All Rights Reserved.

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

					SolveContact(contact);

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

		public static void CheckContact(Contact Contact)
		{

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
	}
}