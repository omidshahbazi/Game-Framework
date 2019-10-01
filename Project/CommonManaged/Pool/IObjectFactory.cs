// Copyright 2019. All Rights Reserved.
namespace GameFramework.Common.Pool
{
	public interface IObjectFactory<T> where T : class, IObject
	{
		//void AfterSendToPool(T Object); ?
		//void BeforeGetFromPool(T Object, object UserData = null); ?

		T Instantiate(object UserData = null);

		void Destroy(T Object);
	}
}