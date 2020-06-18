// Copyright 2019. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace GameFramework.ASCIISerializer.JSONSerializer
{
	class JSONSerializeArray : ISerializeArray
	{
		private List<object> items = new List<object>();

		uint ISerializeData.Count
		{
			get { return (uint)items.Count; }
		}

		string ISerializeData.Name
		{
			get { return ""; }
		}

		public ISerializeData Parent
		{
			get;
			private set;
		}

		string ISerializeData.Content
		{
			get
			{
				StringBuilder str = new StringBuilder();
				str.Append('[');
				for (int i = 0; i < items.Count; ++i)
				{
					if (i != 0)
						str.Append(',');

					object item = items[i];

					if (item == null)
						str.Append("null");
					else if (item is ISerializeData)
						str.Append(((ISerializeData)item).Content);
					else if (item is string)
					{
						str.Append('"');
						str.Append(item.ToString().Replace("\"", "\\\""));
						str.Append('"');
					}
					else if (item is bool)
						str.Append(item.ToString().ToLower());
					else
						str.Append(item.ToString());
				}
				str.Append(']');
				return str.ToString();
			}
		}

		ISerializeData ISerializeData.Parent
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object ISerializeArray.this[uint Index]
		{
			get { return items[(int)Index]; }
			set { items[(int)Index] = value; }
		}

		public JSONSerializeArray(ISerializeData Parent)
		{
			this.Parent = Parent;
		}

		ISerializeArray ISerializeArray.AddArray()
		{
			ISerializeArray obj = new JSONSerializeArray(this);

			items.Add(obj);

			return obj;
		}

		ISerializeObject ISerializeArray.AddObject()
		{
			ISerializeObject obj = new JSONSerializeObject(this);

			items.Add(obj);

			return obj;
		}

		void ISerializeArray.Add(float Item)
		{
			items.Add(Item);
		}

		void ISerializeArray.Add(string Item)
		{
			items.Add(Item);
		}

		void ISerializeArray.Add(double Item)
		{
			items.Add(Item);
		}

		void ISerializeArray.Add(uint Item)
		{
			items.Add(Item);
		}

		void ISerializeArray.Add(int Item)
		{
			items.Add(Item);
		}

		void ISerializeArray.Add(bool Item)
		{
			items.Add(Item);
		}

		void ISerializeArray.Add(object Item)
		{
			items.Add(Item);
		}

		void ISerializeArray.Add(Enum Item)
		{
			items.Add(Item.ToString());
		}

		T ISerializeArray.Get<T>(uint Index, T DefaultValue)
		{
			if (Index >= items.Count)
				return DefaultValue;

			object obj = items[(int)Index];

			if (obj is T)
				return (T)obj;

			Type type = typeof(T);

			if (type.IsEnum)
			{
				if (!Enum.IsDefined(type, obj))
					return DefaultValue;

				obj = (obj is string ? Enum.Parse(type, obj.ToString()) : Enum.ToObject(type, obj));
			}
			else
			{
				try
				{
					obj = Convert.ChangeType(obj, typeof(T));
				}
				catch
				{
					return DefaultValue;
				}
			}

			return (T)obj;
		}

		void ISerializeArray.Clear()
		{
			items.Clear();
		}

		void ISerializeArray.AddRange<T>(T[] Range)
		{
			for (int i = 0; i < Range.Length; ++i)
				((ISerializeArray)this).Add(Range[i]);
		}

		T[] ISerializeArray.GetRange<T>()
		{
			return ((ISerializeArray)(this)).GetRange<T>(0, (uint)items.Count);
		}

		T[] ISerializeArray.GetRange<T>(uint Index, uint Count)
		{
			T[] range = new T[Count];

			for (uint i = 0; i < Count; ++i)
				range[i] = ((ISerializeArray)this).Get<T>(i);

			return range;
		}

		void ISerializeArray.Remove(uint Index)
		{
			items.RemoveAt((int)Index);
		}

		bool ISerializeArray.Contains(object Value)
		{
			return items.Contains(Value);
		}

		IEnumerator<object> ISerializeArray.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		ISerializeArray ISerializeArray.Clone()
		{
			return Creator.Create<ISerializeArray>(((ISerializeData)this).Content);
		}
	}
}
