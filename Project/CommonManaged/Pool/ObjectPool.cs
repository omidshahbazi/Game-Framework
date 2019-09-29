// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
namespace Zorvan.Framework.Common.Pool
{
	public class ObjectPool<T, U>
		where T : class, IObject
		where U : ObjectPool<T>, new()
	{
		private static U instance = null;
		private IObjectHolder<T> objectHolder = null;

		public static U Instance
		{
			get
			{
				if (instance == null)
					instance = new U();

				return instance;
			}
		}

		public IObjectHolder<T> ObjectHolder
		{
			get
			{
				if (objectHolder == null)
					objectHolder = new DefaultObjectHolder<T>();

				return objectHolder;
			}
			set { objectHolder = value; }
		}

		public IObjectFactory<T> Factory
		{
			get;
			set;
		}

		public T Pull(object UserData = null)
		{
			T obj = null;

			obj = ObjectHolder.Pull(UserData);

			if (obj == null)
			{
				if (Factory != null)
					obj = Factory.Instantiate(UserData);
			}
			else
			{
				//obj.GoOutOfPool();
				//Factory.BeforeGetFromPool(obj, UserData);
			}

			return obj;
		}

		public void Push(T Object)
		{
			ObjectHolder.Push(Object);
			//Object.GoInPool();
			//Factory.AfterSendToPool(Object);
		}

		public void Reserve(uint Count, object UserData = null)
		{
			for (uint i = 0; i < Count; ++i)
				Push(Pull(UserData));
		}

		public void Clear()
		{
			objectHolder.Clear();
		}
	}

	public class ObjectPool<T> : ObjectPool<T, ObjectPool<T>> where T : class, IObject
	{ }
}