// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;
using System.Reflection;

namespace Zorvan.Framework.Serialization
{
	public interface ICompileStrategy
	{
		MethodBase GetInstantiator(Type Type);
		MethodInfo GetPreSerialize(Type Type);
		MethodInfo GetPostSerialize(Type Type);
		MethodInfo GetPreDeserialize(Type Type);
		MethodInfo GetPostDeserialize(Type Type);

		MemberInfo[] GetMembers(Type Type);

		int GetMemberID(MemberInfo Member, int DefaultID);
		string GetMemberDefaultValue(MemberInfo Member);
		string GetInstantiatorParameterDefaultValue(MethodBase Method, uint Index);
	}
}
