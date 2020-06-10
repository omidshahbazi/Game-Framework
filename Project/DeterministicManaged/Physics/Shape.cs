// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics
{
	public abstract class Shape : IVisitee
	{
		public virtual void Visit(IVisitor Visitor)
		{
		}
	}
}