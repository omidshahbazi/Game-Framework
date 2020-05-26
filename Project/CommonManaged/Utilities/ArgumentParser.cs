// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace GameFramework.Common.Utilities
{
	public class ArgumentParser
	{
		private class ValueMap : Dictionary<string, string>
		{ }

		private ValueMap values = null;

		public string Content
		{
			get
			{
				StringBuilder builder = new StringBuilder();

				ValueMap.Enumerator it = values.GetEnumerator();
				while (it.MoveNext())
				{
					builder.Append('-');
					builder.Append(it.Current.Key);

					builder.Append(' ');

					string value = it.Current.Value;
					bool containsWhiteSpace = value.Contains(" ");

					if (containsWhiteSpace)
						builder.Append('\"');

					builder.Append(value);

					if (containsWhiteSpace)
						builder.Append('\"');

					builder.Append(' ');
				}

				return builder.ToString();
			}
		}

		public ArgumentParser()
		{
			values = new ValueMap();
		}

		public T Get<T>(string Key, T Default = default(T))
		{
			if (!values.ContainsKey(Key))
				return Default;

			string value = values[Key];

			Type type = typeof(T);

			if (type.IsEnum)
				return (T)Enum.Parse(type, value);

			return (T)Convert.ChangeType(value, type);
		}

		public void Set<T>(string Key, T Value)
		{
			values[Key] = Value.ToString();
		}

		public bool Contains(string Key)
		{
			return values.ContainsKey(Key);
		}

		public static ArgumentParser Parse(string[] Arguments)
		{
			ArgumentParser args = new ArgumentParser();

			if (Arguments == null)
				return args;

			for (int i = 0; i < Arguments.Length; i += 2)
			{
				if (i + 1 >= Arguments.Length)
					break;

				string key = Arguments[i];

				if (!key.StartsWith("-"))
					throw new ArgumentException("Arguemnt should start with '-'", key);

				args.values[key.Substring(1)] = Arguments[i + 1];
			}

			return args;
		}
	}
}