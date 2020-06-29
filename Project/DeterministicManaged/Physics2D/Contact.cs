// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;

namespace GameFramework.Deterministic.Physics2D
{
	public class Contact
	{
		public Body BodyA;
		public Body BodyB;
		public Vector2 Normal;
		public Vector2[] Points;
	}

	public class ContactList : List<Contact>
	{ }
}