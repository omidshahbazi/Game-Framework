// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Mathematics;
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
		void VisitBounds(Bounds Bounds);
		void VisitIdentifier(Identifier Identifier);
	}
}