// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.BinarySerializer
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class KeyAttribute : Attribute
	{
		public uint ID
		{
			get;
			private set;
		}

		public KeyAttribute(uint ID)
		{
			this.ID = ID;
		}
	}
}
