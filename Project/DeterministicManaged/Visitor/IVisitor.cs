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

		void VisitIdentifier(Identifier Identifier);
	}
}
