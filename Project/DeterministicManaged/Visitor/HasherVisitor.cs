// Copyright 2019. All Rights Reserved.
using System;
using System.Collections;
using GameFramework.Common.Utilities;
using GameFramework.Deterministic.Mathematics;

namespace GameFramework.Deterministic.Visitor
{
	public class HasherVisitor : IVisitor
	{
		public int Value
		{
			get;
			private set;
		}

		protected void AddBytes(params byte[] Bytes)
		{
			Value += Get(Bytes);
		}

		protected void AddFloat32(float Value)
		{
		}

		public void Reset()
		{
			Value = 0;
		}

		public void BeginVisitArray(ICollection Collection)
		{
		}

		public void EndVisitArray()
		{
		}

		public void BeginVisitArrayElement()
		{
		}

		public void EndVisitArrayElement()
		{
		}

		public void VisitBool(bool Bool)
		{
			Value += Get(BitConverter.GetBytes(Bool));
		}

		public void VisitInt32(int Int)
		{
			Value += Get(BitConverter.GetBytes(Int));
		}

		public void VisitNumber(Number Number)
		{
			Value += Get(BitConverter.GetBytes(Number.Value));
		}

		public void VisitVector2(Vector2 Vector2)
		{
			VisitNumber(Vector2.X);
			VisitNumber(Vector2.Y);
		}

		public void VisitBounds(Bounds Bounds)
		{
			VisitVector2(Bounds.Position);
			VisitVector2(Bounds.Size);
		}

		public void VisitIdentifier(Identifier Identifier)
		{
			VisitInt32(Identifier);
		}

		private static int Get(byte[] Value)
		{
			return (int)CRC32.CalculateHash(Value);
		}
	}
}