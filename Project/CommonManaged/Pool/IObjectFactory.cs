// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
namespace Zorvan.Framework.Common.Pool
{
	public interface IObjectFactory<T> where T : class, IObject
	{
		//void AfterSendToPool(T Object); ?
		//void BeforeGetFromPool(T Object, object UserData = null); ?

		T Instantiate(object UserData = null);

		void Destroy(T Object);
	}
}