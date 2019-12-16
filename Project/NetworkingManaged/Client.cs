// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Timing;
using System.Collections.Generic;
using System.Net;

namespace GameFramework.Networking
{
	public abstract class Client
	{
		public virtual bool IsReady
		{
			get { return (Time.CurrentEpochTime - Statistics.LastTouchTime < Constants.PING_TIME * 2); }
		}

		public NetworkingStatistics Statistics
		{
			get;
			private set;
		}

		public abstract IPEndPoint EndPoint
		{
			get;
		}

		public Client()
		{
			Statistics = new NetworkingStatistics();
		}
	}

	public class ClientList : List<Client>
	{ }
}