// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer;
using GameFramework.Common.Web;
using System;
using System.Diagnostics;

namespace GameFramework.CommonAPIManaged.PurchaseValidation
{
	public class CafeBazaarPurchaseValidator : IPurchaseValidator
	{
		private const string TOKEN_URL = "https://pardakht.cafebazaar.ir/devapi/v2/auth/token/";
		private const string VALIDATE_URL = "https://pardakht.cafebazaar.ir/devapi/v2/api/validate/{0}/inapp/{1}/purchases/{2}/?access_token={3}";
		private const string AUTHORIZE_URL = "https://pardakht.cafebazaar.ir/devapi/v2/auth/authorize/";

		private string packageName;
		private string clientID;
		private string clientSecret;
		private string refreshToken;
		private string accessToken;

		public CafeBazaarPurchaseValidator(string PackageName, string ClientID, string ClientSecret, string RefreshToken)
		{
			packageName = PackageName;
			clientID = ClientID;
			clientSecret = ClientSecret;
			refreshToken = RefreshToken;

			GetAccessToken();
		}

		private void GetAccessToken()
		{
			try
			{
				Requests.ParameterMap parameters = new Requests.ParameterMap();
				parameters["grant_type"] = "refresh_token";
				parameters["client_id"] = clientID;
				parameters["client_secret"] = clientSecret;
				parameters["refresh_token"] = refreshToken;

				string data = Requests.Post(TOKEN_URL, null, parameters);

				ISerializeObject obj = Creator.Create<ISerializeObject>(data);

				accessToken = obj["access_token"].ToString();
			}
			catch
			{
			}
		}

		public void Validate(uint Price, string SKU, string Token, Action<bool, string> Callback)
		{
			try
			{
				string data = Requests.Get(string.Format(VALIDATE_URL, packageName, SKU, Token, accessToken));

				ISerializeObject obj = Creator.Create<ISerializeObject>(data);

				bool isOK = (Convert.ToInt32(obj["consumptionState"]) == 0 && Convert.ToInt32(obj["purchaseState"]) == 0);

				Callback(isOK, "");
			}
			catch (Exception ex)
			{
				Callback(false, ex.ToString());
			}
		}

		public static string GetRefreshToken(string Code, string ClientID, string ClientSecret, string RedirectURI)
		{
			Requests.ParameterMap parameters = new Requests.ParameterMap();
			parameters["grant_type"] = "authorization_code";
			parameters["code"] = Code;
			parameters["client_id"] = ClientID;
			parameters["client_secret"] = ClientSecret;
			parameters["redirect_uri"] = RedirectURI;

			string data = Requests.Post(TOKEN_URL, null, parameters);

			ISerializeObject obj = Creator.Create<ISerializeObject>(data);

			if (obj.Contains("refresh_token"))
				return obj.Get<string>("refresh_token");

			return "";
		}

		public static void OpenGetCodeURL(string RedirectURI, string ClientID)
		{
			string url = AUTHORIZE_URL + "?response_type=code&access_type=offline&redirect_uri=" + RedirectURI + "&client_id=" + ClientID;

			Process.Start(url);
		}
	}
}