using GameFramework.ASCIISerializer;
using GameFramework.BinarySerializer;
using GameFramework.Common.Extensions;
using GameFramework.Deterministic;
using GameFramework.Deterministic.Physics2D;
using System;
using System.Reflection;
using System.Threading;

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
			Number xx = 24;
		}



		enum en
		{
			a1,
			a2
		}

		class test1
		{
			Number xx = 24;
		}

		class test
		{
			public int[][] b = new int[][] { new int[] { 10, 11, 12 }, new int[] { 20, 30, 40 } };
			public int[,] a = new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };

			Number xx = 242343;
		}

		static void Main(string[] args)
		{
			long num = 1000;
			object obj = Activator.CreateInstance(typeof(Number), num);

			test t = new test();

			ISerializeObject d = Creator.Serialize<ISerializeObject>(t);

			test f = Creator.Bind<test>(Creator.Create<ISerializeObject>(d.Content));

			BufferStream buff = new BufferStream(new byte[10000]);
			Serializer.Serialize(f, buff);

			buff.ResetRead();

			f = Serializer.Deserialize<test>(buff);
		}
	}
}
