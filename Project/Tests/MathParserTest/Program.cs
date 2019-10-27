using System;
using System.Text;
using GameFramework.Analytics;
using GameFramework.ASCIISerializer;
using GameFramework.Common.FileLayer;
using GameFramework.DatabaseManaged;
using GameFramework.DatabaseManaged.Generator;
using GameFramework.MathParser;

namespace MathParserTest
{
	class Program
	{
		static void Main(string[] args)
		{
			FileSystem.DataPath = "D:/";
			ISerializeObject objBase = Creator.Create<ISerializeObject>(FileSystem.Read("base.json"));
			ISerializeObject sec = Creator.Create<ISerializeObject>(FileSystem.Read("sec.json"));

			Creator.Override(sec, objBase);


			//			string str = @"Trophy+
			//if(B1=-1,0,if(B1=0,-30,if(B1=1,-20,if(B1=2,0,20))))+
			//if(B2=-1,0,if(B2=0,-30,if(B2=1,-20,if(B2=2,0,20))))+
			//if(B3=-1,0,if(B3=0,-30,if(B3=1,-20,if(B3=2,0,20))))+
			//if(B4=-1,0,if(B4=0,-30,if(B4=1,-20,if(B4=2,0,20))))+

			//if(B5=-1,0,if(B5=0,-30,if(B5=1,-20,if(B5=2,0,20))))";

			//			Expression ex0 = new Expression(str);
			//			//Expression ex1 = new Expression("  ( 15+ 2)");
			//			//Expression ex2 = new Expression("((1 +2) * 3)");
			//			//Expression ex3 = new Expression("((15 +2) * 3.4) ^ 2");
			//			//Expression ex4 = new Expression("((15 +2 - 1) * 3.4) ^ 0.95");
			//			//Expression ex5 = new Expression("if (x>= \n10, x+10, if (x> 0, 15, 11))");
			//			//Expression ex6 = new Expression("random(1,5)");
			//			double res0 = ex0.Calculate(new Argument("Trophy", 351), new Argument("B1", -1), new Argument("B2", -1), new Argument("B3", -1), new Argument("B4", -1), new Argument("B5", -1));
			//			//double res1 = ex1.Calculate();
			//			//double res2 = ex2.Calculate();
			//			//double res3 = ex3.Calculate();
			//			//double res4 = ex4.Calculate();
			//			//double res5 = ex5.Calculate(new Argument("x", -5));
			//			//double a = ex6.Calculate(new Argument("x", 1));
			//			//Console.WriteLine(ex6.ToString());
			//			Console.ReadLine();
		}
	}
}
