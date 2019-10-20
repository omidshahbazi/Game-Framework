// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer;
using GameFramework.Common.Web;
using System.Net;
using System.Text;

namespace GameFramework.CommonAPIManaged.PushNotification
{
	public class PusheAPI : IPushNotificationAPI
	{
		private static string URL = "https://panel.pushe.co/api/v1/notifications";

		private string packageName;
		private string restAPIKey;

		public PusheAPI(string PackageName, string RestAPIKey)
		{
			packageName = PackageName;
			restAPIKey = RestAPIKey;
		}

		public void Send(string Title, string Message, Segments[] Segments)
		{
			ISerializeObject messageObj = BuildMessage(Title, Message);

			SendMessage(messageObj);
		}

		public void Send(string Title, string Message, params string[] PlayerID)
		{
			if (PlayerID == null || PlayerID.Length == 0)
			{
				Send(Title, Message, new Segments[] { Segments.All });
				return;
			}

			ISerializeObject messageObj = BuildMessage(Title, Message);

			ISerializeObject filter = messageObj.AddObject("filter");

			ISerializeArray pushe_id = filter.AddArray("pushe_id");

			pushe_id.AddRange(PlayerID);

			SendMessage(messageObj);
		}

		private ISerializeObject BuildMessage(string Title, string Message)
		{
			ISerializeObject messageObj = Creator.Create<ISerializeObject>();

			ISerializeArray applications = messageObj.AddArray("applications");
			applications.Add(packageName);

			ISerializeObject notification = messageObj.AddObject("notification");
			notification["content"] = Message;

			if (!string.IsNullOrEmpty(Title))
				notification["title"] = Title;

			return messageObj;
		}

		private string SendMessage(ISerializeObject Message)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

			Requests.HeaderMap headers = new Requests.HeaderMap();
			headers["Authorization"] = "Token " + restAPIKey;
			headers["Content-Type"] = "application/json";
			headers["Accept"] = "application/json";

			return Requests.PostBytes(URL, Encoding.UTF8.GetBytes(Message.Content), headers);
		}
	}
}