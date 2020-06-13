// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.Deterministic.Physics
{
	class Manifold : Contact
	{
		public Number Penetration;
		public Number MixedRestitution;
		public Number MixedDynamicFriction;
		public Number MixedStaticFriction;
	}

	class ManifoldList : List<Manifold>
	{ }
}