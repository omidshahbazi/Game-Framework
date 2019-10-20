// Copyright 2019. All Rights Reserved.
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;

namespace GameFramework.Common.Web
{
	public static class Requests
	{
		public class HeaderMap : Dictionary<string, string>
		{ }

		public class ParameterMap : Dictionary<string, string>
		{ }

		public static string Get(string URI, HeaderMap Headers = null)
		{
			WebClient client = new WebClient();

			FillHeaders(client, Headers);

			return client.DownloadString(URI);
		}

		public static string PostString(string URI, string Data = "", HeaderMap Headers = null)
		{
			WebClient client = new WebClient();

			FillHeaders(client, Headers);

			return client.UploadString(URI, Data);
		}

		public static string PostBytes(string URI, byte[] Data, HeaderMap Headers = null)
		{
			WebClient client = new WebClient();

			FillHeaders(client, Headers);

			return client.Encoding.GetString(client.UploadData(URI, Data));
		}

		public static string Post(string URI, HeaderMap Headers = null, ParameterMap Parameters = null)
		{
			WebClient client = new WebClient();

			FillHeaders(client, Headers);

			NameValueCollection parameters = BuildParameters(Parameters);

			return client.Encoding.GetString(client.UploadValues(URI, "POST", parameters));
		}

		private static void FillHeaders(WebClient Client, HeaderMap Headers)
		{
			if (Headers == null)
				return;

			HeaderMap.Enumerator it = Headers.GetEnumerator();
			while (it.MoveNext())
				Client.Headers.Add(it.Current.Key, it.Current.Value);
		}

		private static NameValueCollection BuildParameters(ParameterMap Parameters)
		{
			if (Parameters == null)
				return null;

			NameValueCollection parameters = new NameValueCollection();

			HeaderMap.Enumerator it = Parameters.GetEnumerator();
			while (it.MoveNext())
				parameters[it.Current.Key] = it.Current.Value;

			return parameters;
		}
	}
}
