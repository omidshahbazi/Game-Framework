// Copyright 2019. All Rights Reserved.
using System.Collections;

namespace GameFramework.Deterministic.Visitor
{
	public interface IVisitor
	{
		void Reset();

		void BeginVisitArray(ICollection Collection);
		void EndVisitArray();

		void BeginVisitArrayElement();
		void EndVisitArrayElement();

		void VisitBool(bool Bool);
		void VisitInt32(int Int);

		void VisitNumber(Number Number);
		void VisitVector2(Vector2 Vector2);
		void VisitVector3(Vector3 Vector3);
		void VisitMatrix2(Matrix2 Matrix2);
		void VisitMatrix3(Matrix3 Matrix3);
		void VisitBounds(Bounds Bounds);
		void VisitIdentifier(Identifier Identifier);
	}
}