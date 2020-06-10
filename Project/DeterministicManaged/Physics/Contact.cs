// Copyright 2019. All Rights Reserved.
using GameFramework.Deterministic.Visitor;

namespace GameFramework.Deterministic.Physics
{
	public class Contact
	{
		public Body BodyA;
		public Body BodyB;
		public Vector3 Normal;
		public Vector3[] Points;
	}
}