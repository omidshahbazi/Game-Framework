using GameFramework.ASCIISerializer;
using GameFramework.BinarySerializer;
using GameFramework.Common.Extensions;
using GameFramework.Deterministic;
using GameFramework.Deterministic.Physics;
using System;
using System.Threading;

namespace MathParserTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Scene scene = new Scene();

			Body body1 = new Body();
			body1.Mass = 1;
			body1.Position = new Vector3(0, 2, 0);
			body1.Orientation = Matrix3.Identity;
			//body1.Shape = new SphereShape() { Radius = 0.5F};

			body1.Shape = new PolygonShape()
			{
				Vertices = new Vector3[] { new Vector3(-0.5F, -0.5F, 0), new Vector3(-0.5F, 0.5F, 0), new Vector3(0.5F, 0.5F, 0), new Vector3(0.5F, -0.5F, 0) },
				Normals = new Vector3[] { Vector3.One, Vector3.One, Vector3.One, Vector3.One }
			};

			ArrayUtilities.Add(ref scene.Bodies, body1);

			Body body2 = new Body();
			body2.Mass = 0;
			body2.Position = new Vector3(0, 0, 0);
			body2.Orientation = Matrix3.Identity;
			body2.Shape = new SphereShape() { Radius = 1 };
			ArrayUtilities.Add(ref scene.Bodies, body2);

			Simulation.Config config = new Simulation.Config();
			config.StepTime = 0.2F;

			while (true)
			{
				ContactList contacts = new ContactList();

				Simulation.Simulate(scene, config, contacts);

				Console.WriteLine("Body1: {0}, Body2: {1} [Contacts: {2}]", body1.Position, body2.Position, contacts.Count);

				Thread.Sleep(1000 * config.StepTime);
			}
		}




		//	class obj1
		//	{
		//		int a;
		//	}

		//	enum infos
		//	{
		//		element1,
		//		element2
		//	}

		//	class Obj
		//	{
		//		int a = 0;
		//		public string test
		//		{
		//			get;
		//			private set;
		//		}

		//		infos Info;

		//		obj1[] vals;
		//	}



		//	enum en
		//	{
		//		a1,
		//		a2
		//	}

		//	class test1
		//	{
		//		float xx = 23.5F;
		//	}

		//class test
		//	{
		//		public int[][] b = new int[][] { new int[] { 10, 11, 12 }, new int[] { 20, 30, 40 } };
		//		public int[,] a = new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
		//		public Matrix3 i = Matrix3.Zero;
		//		public Matrix3 j = Matrix3.Identity;
		//	}

		//	static void Main(string[] args)
		//	{
		//		test t = new test();

		//		ISerializeObject d = Creator.Serialize<ISerializeObject>(t);

		//		test f = Creator.Bind<test>(d);

		//		BufferStream buff = new BufferStream(new byte[10000]);
		//		Serializer.Serialize(f, buff);

		//		buff.ResetRead();

		//		f = Serializer.Deserialize<test>(buff);
		//	}
	}
}
