// Copyright 2019. All Rights Reserved.

namespace GameFramework.CommonAPIManaged.PushNotification
{
	public enum Segments
	{
		All,
		Active_Users,
		Inactive_Users,
		Engaged_Users
	}

	public interface IPushNotificationAPI
	{
		void Send(string Title, string Message, Segments[] Segments);
		void Send(string Title, string Message, params string[] PlayerID);
	}
}