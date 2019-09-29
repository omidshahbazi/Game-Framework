// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Net;
using Zorvan.Framework.Common.SystemController;

namespace Zorvan.Framework.Common.WebRequest
{
	public class WebRequestManager : ISystem
	{
		private class RequestState
		{
			public string URL
			{
				get;
				private set;
			}

			public System.Action<byte[]> OnComplete
			{
				get;
				private set;
			}

			public System.Action<System.Exception> OnFailed
			{
				get;
				private set;
			}

			public int RetryCount
			{
				get;
				set;
			}

			public byte[] Result
			{
				get;
				set;
			}

			public System.Exception Error
			{
				get;
				set;
			}

			public RequestState(string URL, System.Action<byte[]> OnComplete, System.Action<System.Exception> OnFailed, int RetryCount)
			{
				this.URL = URL;
				this.OnComplete = OnComplete;
				this.OnFailed = OnFailed;
				this.RetryCount = RetryCount;
			}

			public void HandleCallbacks()
			{
				if (Error == null)
					OnComplete(Result);
				else if (OnFailed != null)
					OnFailed(Error);
			}
		}

		private const int MAX_CLIENT = 1;

		private int totalWebClient = 0;
		private List<RequestState> inProgressStates = new List<RequestState>();
		private List<RequestState> finishedStates = new List<RequestState>();
		private List<WebClient> freeClients = new List<WebClient>();
		private Dictionary<WebClient, RequestState> inProgressRequests = new Dictionary<WebClient, RequestState>();

		void ISystem.Initialize(params object[] Arguments)
		{
			throw new NotImplementedException();
		}

		void ISystem.Uninitialize(params object[] Arguments)
		{
			throw new NotImplementedException();
		}

		void ISystem.Update(params object[] Arguments)
		{
		}
		//	base.Update();

		//	RequestState state = null;

		//	for (int i = 0; i < finishedStates.Count; ++i)
		//		finishedStates[i].HandleCallbacks();
		//	finishedStates.Clear();

		//	if (inProgressStates.Count == 0)
		//		return;

		//	WebClient client = GetWebClient();

		//	if (client == null)
		//		return;

		//	state = inProgressStates[0];
		//	inProgressStates.RemoveAt(0);

		//	client.DownloadDataAsync(new System.Uri(state.URL));

		//	inProgressRequests[client] = state;
		//}

		private WebClient GetWebClient()
		{
			WebClient client = null;
			if (freeClients.Count != 0)
			{
				client = freeClients[0];
				freeClients.RemoveAt(0);
				client.CancelAsync();
				return client;
			}

			if (totalWebClient == MAX_CLIENT)
				return client;

			totalWebClient++;

			client = new WebClient();
			client.DownloadDataCompleted += DownloadDataCompleted;
			return client;
		}

		private void DownloadDataCompleted(object Sender, DownloadDataCompletedEventArgs Args)
		{
			WebClient client = (WebClient)Sender;

			freeClients.Add(client);

			RequestState state = inProgressRequests[client];
			inProgressRequests.Remove(client);

			if (Args.Error != null)
			{
				if (state.RetryCount != 0)
				{
					state.RetryCount--;

					inProgressStates.Add(state);

					return;
				}

				state.Error = Args.Error;
			}

			state.Result = Args.Result;
			finishedStates.Add(state);
		}

		public void DownloadData(string URL, System.Action<byte[]> OnComplete, System.Action<System.Exception> OnFailed, int RetryCount = 0)
		{
			inProgressStates.Add(new RequestState(URL, OnComplete, OnFailed, RetryCount));
		}
	}
}