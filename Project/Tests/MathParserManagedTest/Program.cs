using System;
using System.Text;
using System.Threading;
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
		class obj1
		{
			int a;
		}

		enum infos
		{
			element1,
			element2
		}

		class Obj
		{
			int a = 0;
			public string test
			{
				get;
				private set;
			}

			infos Info;

			obj1[] vals;
		}

		static void Main(string[] args)
		{
			Obj obj = Creator.Create<Obj>("{\"a\":10,\"test\":null,\"vals\":[{\"a\":1}, {\"a\":4}, {\"a\":6}], \"Info\":\"element2\"}");
			int[] b = Creator.Create<int[]>("[1, 3, 5]");

			ISerializeObject d = Creator.Serialize<ISerializeObject>(obj);

			FileSystem.DataPath = "D:\\";
			if (!FileSystem.DirectoryExists("omid"))
				FileSystem.CreateDirectory("omid");


			MySQLDatabase db = new MySQLDatabase("localhost", "root", "!QAZ2wsx", "backgammon");

			int i = db.ExecuteInsert("INSERT INTO users_game(type, bet, white_user_id, black_user_id, bot_user_info, winner_user_id, finish_reason, start_time, end_time, version, replay_data) VALUES(1, 1, 1, 1, NULL, 1, NULL, NOW(), NULL, 1, NULL)");
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
