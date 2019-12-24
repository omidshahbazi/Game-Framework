// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GameFramework.Common.Utilities
{
	public static class ReflectionExtensions
	{
		public const BindingFlags PrivateStaticFlags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;
		public const BindingFlags PublicStaticFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
		public const BindingFlags AllNonStaticFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		public static MemberType[] GetFields<MemberType>(this Type Type, BindingFlags Flags = PublicStaticFlags)
		{
			List<MemberType> members = new List<MemberType>();

			FieldInfo[] fields = Type.GetFields(Flags);

			for (int i = 0; i < fields.Length; ++i)
			{
				FieldInfo field = fields[i];

				if (field.FieldType != typeof(MemberType))
					continue;

				members.Add((MemberType)field.GetValue(null));
			}

			return members.ToArray();
		}

		public static MemberInfo[] GetMemberVariables(this Type Type, BindingFlags Flags = PublicStaticFlags)
		{
			List<MemberInfo> members = new List<MemberInfo>();

			members.AddRange(Type.GetFields(Flags));
			members.AddRange(Type.GetProperties(Flags));

			for (int i = 0; i < members.Count; ++i)
			{
				MemberInfo member = members[i];

				object[] attribs = member.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true);
				if (attribs == null || attribs.Length == 0)
					continue;

				members.RemoveAt(i--);
			}

			return members.ToArray();
		}

		public static uint GetSizeOf(this Type Type)
		{
			if (Type == typeof(bool))
				return sizeof(bool);

			if (Type == typeof(char))
				return sizeof(bool);

			if (Type == typeof(short))
				return sizeof(short);

			if (Type == typeof(int))
				return sizeof(int);

			if (Type == typeof(long))
				return sizeof(long);

			if (Type == typeof(ushort))
				return sizeof(ushort);

			if (Type == typeof(uint))
				return sizeof(uint);

			if (Type == typeof(ulong))
				return sizeof(ulong);

			if (Type == typeof(float))
				return sizeof(float);

			if (Type == typeof(double))
				return sizeof(double);

			return 0;
		}
	}
}