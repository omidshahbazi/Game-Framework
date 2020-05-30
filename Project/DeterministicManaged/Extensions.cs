// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
using GameFramework.Deterministic.Mathematics;
using GameFramework.Deterministic.Visitor;
using System;

namespace GameFramework.Deterministic
{
	public static class Extensions
	{
		public static Identifier ReadIdentifier(this BufferStream Buffer)
		{
			return new Identifier(Buffer.ReadInt32());
		}

		public static Number ReadNumber(this BufferStream Buffer)
		{
			return Buffer.ReadFloat32();
		}

		public static Vector2 ReadVector2(this BufferStream Buffer)
		{
			Vector2 data = new Vector2();

			data.X = Buffer.ReadNumber();
			data.Y = Buffer.ReadNumber();

			return data;
		}

		public static Bounds ReadBounds(this BufferStream Buffer)
		{
			Bounds data = new Bounds();

			data.Position = Buffer.ReadVector2();
			data.Size = Buffer.ReadVector2();

			return data;
		}

		public static void VisitArray(this IVisitor Visitor, IVisitee[] Visitees)
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