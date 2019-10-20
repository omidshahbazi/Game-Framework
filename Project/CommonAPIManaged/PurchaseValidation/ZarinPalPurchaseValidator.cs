// Copyright 2019. All Rights Reserved.
using GameFramework.ASCIISerializer;
using GameFramework.Common.Web;
using System;

namespace GameFramework.CommonAPIManaged.PurchaseValidation
{
	public class ZarinPalPurchaseValidator : IPurchaseValidator
	{
		private const string VALIDATE_URL = "https://www.zarinpal.com/pg/rest/WebGate/PaymentVerification.json";

		private string merchantID;

		public ZarinPalPurchaseValidator(string MerchantID)
		{
			merchantID = MerchantID;
		}

		public void Validate(uint Price, string SKU, string Token, Action<bool, string> Callback)
		{
			try
			{
				Requests.HeaderMap headers = new Requests.HeaderMap();
				headers["Content-Type"] = "application/json";

				ISerializeObject dataObj = Creator.Create<ISerializeObject>();
				dataObj["MerchantID"] = merchantID;
				dataObj["Authority"] = Token;
				dataObj["Amount"] = Price;

				string data = Requests.PostString(VALIDATE_URL, dataObj.Content, headers);

				ISerializeObject obj = Creator.Create<ISerializeObject>(data);

				bool isOK = (Convert.ToInt32(obj["Status"]) == 100);

				Callback(isOK, "");
			}
			catch (Exception ex)
			{
				Callback(false, ex.ToString());
			}
		}
	}
}