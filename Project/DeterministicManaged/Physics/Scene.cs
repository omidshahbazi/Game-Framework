// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics
{
	public class Scene : IVisitee
	{
		public Body[] Bodies;

		public void Visit(IVisitor Visitor)
		{
			Visitor.VisitArray(Bodies);
		}
	}
}