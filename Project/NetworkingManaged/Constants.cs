// Copyright 2019. All Rights Reserved.
namespace GameFramework.NetworkingManaged
{
	static class Constants
	{
		public static class Control
		{
			public const int SIZE = sizeof(byte);

			public const byte BUFFER = 1;
			public const byte PING = 2;
		}

		public static class Packet
		{
			public const int HEADER_SIZE = Control.SIZE;
		}
	}
}