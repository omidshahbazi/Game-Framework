// Copyright 2015-2017 Zorvan Game Studio. All Rights Reserved.
using System;
using System.Security.Cryptography;

namespace Zorvan.Framework.Common.Cryptography
{
	public static class HMAC
	{
		public static string Generate(byte[] Data, byte[] Key)
		{
			using (HMACSHA256 hmacSHA256 = new HMACSHA256(Key))
				return Convert.ToBase64String(hmacSHA256.ComputeHash(Data));
		}
	}
}
