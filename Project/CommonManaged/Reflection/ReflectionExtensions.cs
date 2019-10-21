// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameFramework.Common.Utilities
{
	public static class ReflectionExtensions
	{
		public static List<MemberType> GetFields<MemberType>(this Type Type)
		{
			List<MemberType> members = new List<MemberType>();

			FieldInfo[] fields = Type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

			for (int i = 0; i < fields.Length; ++i)
			{
				FieldInfo field = fields[i];

				if (field.FieldType != typeof(MemberType))
					continue;

				members.Add((MemberType)field.GetValue(null));
			}

			return members;
		}
	}
}