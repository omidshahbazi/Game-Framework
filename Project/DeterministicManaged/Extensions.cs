// Copyright 2019. All Rights Reserved.
using GameFramework.BinarySerializer;
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

		public static Vector3 ReadVector3(this BufferStream Buffer)
		{
			Vector3 data = new Vector3();

			data.X = Buffer.ReadNumber();
			data.Y = Buffer.ReadNumber();
			data.Z = Buffer.ReadNumber();

			return data;
		}

		public static Matrix3 ReadMatrix3(this BufferStream Buffer)
		{
			Matrix3 data = new Matrix3();

			for (int i = 0; i < 3; ++i)
				for (int j = 0; j < 3; ++j)
					data.Values[i, j] = Buffer.ReadNumber();

			return data;
		}

		public static Bounds ReadBounds(this BufferStream Buffer)
		{
			Bounds data = new Bounds();

			data.Position = Buffer.ReadVector3();
			data.Size = Buffer.ReadVector3();

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

		public static void VisitArray(this IVisitor Visitor, Number[] Visitees)
		{
			Visitor.BeginVisitArray(Visitees);
			if (Visitees != null)
				for (int i = 0; i < Visitees.Length; ++i)
				{
					Visitor.BeginVisitArrayElement();

					Visitor.VisitNumber(Visitees[i]);

					Visitor.EndVisitArrayElement();
				}
			Visitor.EndVisitArray();
		}

		public static void VisitArray(this IVisitor Visitor, Vector2[] Visitees)
		{
			Visitor.BeginVisitArray(Visitees);
			if (Visitees != null)
				for (int i = 0; i < Visitees.Length; ++i)
				{
					Visitor.BeginVisitArrayElement();

					Visitor.VisitVector2(Visitees[i]);

					Visitor.EndVisitArrayElement();
				}
			Visitor.EndVisitArray();
		}

		public static void VisitArray(this IVisitor Visitor, Vector3[] Visitees)
		{
			Visitor.BeginVisitArray(Visitees);
			if (Visitees != null)
				for (int i = 0; i < Visitees.Length; ++i)
				{
					Visitor.BeginVisitArrayElement();

					Visitor.VisitVector3(Visitees[i]);

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

		public static void Deserialize(this Number[] Visitees, ref Number[] RefVisitees, byte[] Data)
		{
			Visitees.Deserialize(ref RefVisitees, new BufferStream(Data));
		}

		public static void Deserialize(this Number[] Visitees, ref Number[] RefVisitees, BufferStream Buffer)
		{
			uint len = Buffer.BeginReadArray();

			Array.Resize(ref RefVisitees, (int)len);

			for (int i = 0; i < len; ++i)
				RefVisitees[i] = Buffer.ReadNumber();
		}

		public static void Deserialize(this Vector2[] Visitees, ref Vector2[] RefVisitees, byte[] Data)
		{
			Visitees.Deserialize(ref RefVisitees, new BufferStream(Data));
		}

		public static void Deserialize(this Vector2[] Visitees, ref Vector2[] RefVisitees, BufferStream Buffer)
		{
			uint len = Buffer.BeginReadArray();

			Array.Resize(ref RefVisitees, (int)len);

			for (int i = 0; i < len; ++i)
				RefVisitees[i] = Buffer.ReadVector2();
		}

		public static void Deserialize(this Vector3[] Visitees, ref Vector3[] RefVisitees, byte[] Data)
		{
			Visitees.Deserialize(ref RefVisitees, new BufferStream(Data));
		}

		public static void Deserialize(this Vector3[] Visitees, ref Vector3[] RefVisitees, BufferStream Buffer)
		{
			uint len = Buffer.BeginReadArray();

			Array.Resize(ref RefVisitees, (int)len);

			for (int i = 0; i < len; ++i)
				RefVisitees[i] = Buffer.ReadVector3();
		}
	}
}