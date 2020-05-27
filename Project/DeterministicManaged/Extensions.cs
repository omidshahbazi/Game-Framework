// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Deterministic.Visitor;
using System;

namespace GameFramework.Deterministic
{
	public static class Extensions
	{
		public static void Visit(this IVisitee[] Visitees, IVisitor Visitor)
		{
			Visitor.BeginVisitArray(Visitees);
			if (Visitees != null)
				for (int i = 0; i < Visitees.Length; ++i)
				{
					Visitor.BeginVisitArrayElement();

					Visitees[i].Visit(Visitor);

					Visitor.EndVisitArrayElement();
				}
			Visitor.EndVisitArray();
		}

		public static void Deserialize<T>(this T[] Visitees, ref T[] RefVisitees, byte[] Data, Func<BufferStream, T> Deserializer) where T : IVisitee
		{
			Visitees.Deserialize(ref RefVisitees, new BufferStream(Data), Deserializer);
		}

		public static void Deserialize<T>(this T[] Visitees, ref T[] RefVisitees, BufferStream Buffer, Func<BufferStream, T> Deserializer) where T : IVisitee
		{
			uint len = Buffer.BeginReadArray();

			Array.Resize(ref RefVisitees, (int)len);

			for (int i = 0; i < len; ++i)
				RefVisitees[i] = Deserializer(Buffer);
		}
	}
}