// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System.Net.NetworkInformation;

namespace Zorvan.Framework.Common.Utilities
{
	public static class SystemInfo
	{
		public static bool IsNetworkAvailable
		{
			get { return NetworkInterface.GetIsNetworkAvailable(); }
		}

		//http://stackoverflow.com/questions/11705906/programmatically-getting-the-mac-of-an-android-device
		//http://answers.unity3d.com/questions/688797/get-android-wifi-mac-address.html
		//http://stackoverflow.com/questions/4468248/unique-id-of-android-device
		//http://stackoverflow.com/questions/10253893/c-sharp-get-mac-address-with-mono-for-mac-platform
		//http://blog.kibotu.net/java/how-to-get-the-mac-address-on-android
		public static string UniqueDeviceIdentifier
		{
			get
			{
				NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();

				for (int i = 0; i < networks.Length; ++i)
				{
					NetworkInterface network = networks[i];
					
					switch (network.NetworkInterfaceType)
					{
						case NetworkInterfaceType.Ethernet:
						case NetworkInterfaceType.Ethernet3Megabit:
						case NetworkInterfaceType.FastEthernetFx:
						case NetworkInterfaceType.FastEthernetT:
						case NetworkInterfaceType.GigabitEthernet:
						case NetworkInterfaceType.Wireless80211:
							return network.GetPhysicalAddress().ToString();
					}
				}

				throw new System.Exception("Couldn't find UniqueDeviceIdentifier");
			}
		}
	}
}