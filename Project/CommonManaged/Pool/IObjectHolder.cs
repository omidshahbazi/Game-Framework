// Copyright 2019. All Rights Reserved.
namespace Zorvan.Framework.Common.Pool
{
	public interface IObjectHolder<T> where T : class, IObject
	{
		T Pull(object UserData = null);
		void Push(T Object);
		void Clear();
	}
}