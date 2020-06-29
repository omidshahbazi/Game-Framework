// Copyright 2019. All Rights Reserved.
using System;
using System.Collections;
using GameFramework.Common.Utilities;

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
			Value += Get(BitConverter.GetBytes(Number));
		}

		public void VisitVector2(Vector2 Vector2)
		{
			VisitNumber(Vector2.X);
			VisitNumber(Vector2.Y);
		}

		public void VisitVector3(Vector3 Vector3)
		{
			VisitNumber(Vector3.X);
			VisitNumber(Vector3.Y);
			VisitNumber(Vector3.Z);
		}

		public void VisitMatrix2(Matrix2 Matrix2)
		{
			for (int i = 0; i < 2; ++i)
				for (int j = 0; j < 2; ++j)
					VisitNumber(Matrix2.Values[i, j]);
		}

		public void VisitMatrix3(Matrix3 Matrix3)
		{
			for (int i = 0; i < 3; ++i)
				for (int j = 0; j < 3; ++j)
					VisitNumber(Matrix3.Values[i, j]);
		}

		public void VisitBounds(Bounds Bounds)
		{
			VisitVector3(Bounds.Position);
			VisitVector3(Bounds.Size);
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