// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer;
using GameFramework.Common.Web;
using System;

namespace GameFramework.CommonAPIManaged.PurchaseValidation
{
	public class MyketPurchaseValidator : IPurchaseValidator
	{
		private const string VALIDATE_URL = "https://developer.myket.ir/api/applications/{0}/purchases/products/{1}/tokens/{2}";

		private string packageName;
		private string accessToken;

		public MyketPurchaseValidator(string PackageName, string AccessToken)
		{
			packageName = PackageName;
			accessToken = AccessToken;
		}

		public void Validate(uint Price, string SKU, string Token, Action<bool, string> Callback)
		{
			try
			{
				Requests.HeaderMap headers = new Requests.HeaderMap();
				headers["X-Access-Token"] = accessToken;

				string data = Requests.Get(string.Format(VALIDATE_URL, packageName, SKU, Token), headers);

				ISerializeObject obj = Creator.Create<ISerializeObject>(data);

				bool isOK = (Convert.ToInt32(obj["purchaseState"]) == 0);

				Callback(isOK, "");
			}
			catch (Exception ex)
			{
				Callback(false, ex.ToString());
			}
		}
	}
}