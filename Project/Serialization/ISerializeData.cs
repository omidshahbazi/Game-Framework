// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
namespace Zorvan.Framework.Serialization
{
	public interface ISerializeData
	{
		ISerializeData Parent
		{
			get;
		}

		string Name
		{
			get;
		}

		uint Count
		{
			get;
		}

		string Content
		{
			get;
		}
	}
}
