// Copyright 2019. All Rights Reserved.
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
