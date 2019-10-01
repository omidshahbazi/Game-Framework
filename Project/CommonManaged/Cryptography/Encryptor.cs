// Copyright 2019. All Rights Reserved.
using System.Security.Cryptography;

namespace GameFramework.Common.Cryptography
{
	public class Encryptor
	{
		private DESCryptoServiceProvider provider = null;

		public Encryptor()
		{
			provider = new DESCryptoServiceProvider();
		}

		public Encryptor(byte[] Key, byte[] IV)
		{
			provider = new DESCryptoServiceProvider();
			provider.Key = Key;
			provider.IV = IV;
		}

		public byte[] Encrypt(byte[] Data)
		{
			if (Data == null || Data.Length == 0)
				return Data;

			return provider.CreateEncryptor().TransformFinalBlock(Data, 0, Data.Length);
		}

		public byte[] Decrypt(byte[] Data)
		{
			if (Data == null || Data.Length == 0)
				return Data;

			return provider.CreateDecryptor().TransformFinalBlock(Data, 0, Data.Length);
		}
	}
}
