﻿using GameFramework.BinarySerializer;

namespace BinarySerializerManagedTest
{
	class Program
	{
		enum en
		{
			a1,
			a2
		}

		class test
		{
			public int a = 340;
			public string value = null;
			public en e;

			public test child;
		}

		static void Main(string[] args)
		{
			Serializer.RegisterType<test>();

			test t = new test();
			t.value = "hellow world!";
			t.e = en.a2;

			BufferStream buffer = Serializer.Serialize(t);




			//byte[] bytes = new byte[50 * 1024];
			//Serializer test = new Serializer(new System.IO.MemoryStream(bytes));
			////test.WriteInt32(-10);
			////Console.WriteLine(test.ReadInt32());
			//int[] intArray = new int[10] { 257, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			//test.BeginWriteArray(intArray.Length);
			//for (int i = 0; i < intArray.Length; ++i)
			//	test.WriteInt32(intArray[i]);
			//test.EndWriteArray();
			//int len = test.BeginReadArray();

			//Serializer test1 = new Serializer(new System.IO.MemoryStream(bytes));

			//for (int i = 0; i < (a.Length / 4); ++i)
			//{
			//	Console.WriteLine(test1.ReadInt32());
			//}

			//Console.ReadKey();
		}
	}
}