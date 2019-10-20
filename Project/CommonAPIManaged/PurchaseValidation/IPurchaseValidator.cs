// Copyright 2019. All Rights Reserved.
using System;

namespace GameFramework.CommonAPIManaged.PurchaseValidation
{
	public interface IPurchaseValidator
	{
		void Validate(int Price, string SKU, string Token, Action<bool, string> Callback);
	}
}