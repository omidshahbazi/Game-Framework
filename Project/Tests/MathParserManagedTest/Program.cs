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
			body1.Position = new Vector3(0, 10, 0);
			body1.Shape = new CircleShape() { Radius = 2 };
			ArrayUtilities.Add(ref scene.Bodies, body1);

			Body body2 = new Body();
			body2.Mass = 0;
			body2.Position = new Vector3(0, 0, 0);
			body2.Shape = new CircleShape() { Radius = 1 };
			ArrayUtilities.Add(ref scene.Bodies, body2);

			Simulation.Config config = new Simulation.Config();
			config.StepTime = 1;

			ContactList contacts = new ContactList();

			while (true)
			{
				Simulation.Simulate(scene, config, contacts);

				Console.WriteLine("Body1: {0}, Body2: {1}", body1.Position, body2.Position);

				Thread.Sleep(1000);
			}
		}
	}




	//class Program
	//{
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

	//	class test
	//	{
	//		public Number i = 1;
	//	}

	//	static void Main(string[] args)
	//	{
	//		test t = new test();

	//		ISerializeObject d = Creator.Serialize<ISerializeObject>(t);

	//		d.Contains("");
	//		//FrameData f = Creator.Bind<FrameData>(d);


	//		//			string str = @"Trophy+
	//		//if(B1=-1,0,if(B1=0,-30,if(B1=1,-20,if(B1=2,0,20))))+
	//		//if(B2=-1,0,if(B2=0,-30,if(B2=1,-20,if(B2=2,0,20))))+
	//		//if(B3=-1,0,if(B3=0,-30,if(B3=1,-20,if(B3=2,0,20))))+
	//		//if(B4=-1,0,if(B4=0,-30,if(B4=1,-20,if(B4=2,0,20))))+

	//		//if(B5=-1,0,if(B5=0,-30,if(B5=1,-20,if(B5=2,0,20))))";

	//		//			Expression ex0 = new Expression(str);
	//		//			//Expression ex1 = new Expression("  ( 15+ 2)");
	//		//			//Expression ex2 = new Expression("((1 +2) * 3)");
	//		//			//Expression ex3 = new Expression("((15 +2) * 3.4) ^ 2");
	//		//			//Expression ex4 = new Expression("((15 +2 - 1) * 3.4) ^ 0.95");
	//		//			//Expression ex5 = new Expression("if (x>= \n10, x+10, if (x> 0, 15, 11))");
	//		//			//Expression ex6 = new Expression("random(1,5)");
	//		//			double res0 = ex0.Calculate(new Argument("Trophy", 351), new Argument("B1", -1), new Argument("B2", -1), new Argument("B3", -1), new Argument("B4", -1), new Argument("B5", -1));
	//		//			//double res1 = ex1.Calculate();
	//		//			//double res2 = ex2.Calculate();
	//		//			//double res3 = ex3.Calculate();
	//		//			//double res4 = ex4.Calculate();
	//		//			//double res5 = ex5.Calculate(new Argument("x", -5));
	//		//			//double a = ex6.Calculate(new Argument("x", 1));
	//		//			//Console.WriteLine(ex6.ToString());
	//		//			Console.ReadLine();
	//	}
	//}
}
