// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer.JSONSerializer;

namespace GameFramework.ASCIISerializer
{
	public static class Creator
	{
		private static class Overrider
		{
			public static void Override(ISerializeData Data, ISerializeData On)
			{
				if (Data is ISerializeObject)
					Override((ISerializeObject)Data, (ISerializeObject)On);
				else
					Override((ISerializeArray)Data, (ISerializeArray)On);
			}

			public static void Override(ISerializeObject Data, ISerializeObject On)
			{
				var it = Data.GetEnumerator();
				while (it.MoveNext())
				{
					string key = it.Current.Key;
					object value = it.Current.Value;

					object onValue = On.Get<object>(key);

					if (value is ISerializeObject)
					{
						ISerializeObject onObj = null;
						if (onValue != null)
						{
							if (onValue is ISerializeObject)
								onObj = (ISerializeObject)onValue;
							else
							{
								Data.Remove(key);

								it = Data.GetEnumerator();
							}
						}

						if (onObj == null)
							onObj = On.AddObject(key);

						Override((ISerializeData)value, onObj);
					}
					else if (value is ISerializeArray)
					{
						if (onValue != null)
							Data.Remove(key);

						it = Data.GetEnumerator();

						Override((ISerializeData)value, On.AddArray(key));
					}
					else
						On.Set(key, value);
				}
			}

			public static void Override(ISerializeArray Data, ISerializeArray On)
			{
				var it = Data.GetEnumerator();
				while (it.MoveNext())
				{
					object value = it.Current;

					if (value is ISerializeObject)
						Override((ISerializeObject)value, On.AddObject());
					else if (value is ISerializeArray)
						Override((ISerializeArray)value, On.AddArray());
					else
					{
						if (On.Contains(value))
							continue;

						On.Add(value);
					}
				}
			}
		}

		private static readonly JSONParser parser = new JSONParser();

		public static T Create<T>() where T : ISerializeData
		{
			if (typeof(T) == typeof(ISerializeObject))
				return (T)(ISerializeData)new JSONSerializeObject(null);
			else
				return (T)(ISerializeData)new JSONSerializeArray(null);
		}

		public static T Create<T>(string Data) where T : ISerializeData
		{
			return (T)parser.Parse(ref Data);
		}

		public static void Override<T>(T Data, T On) where T : ISerializeData
		{
			Overrider.Override(Data, On);
		}
	}
}