using System;
using System.Security.Cryptography;

namespace DemoButtons
{
	 class MyEncryptionHandler2 : wgssSTU.ITabletEncryptionHandler2
	 {
			RSACryptoServiceProvider rsaProvider;
			RSAParameters rsaParameters;
			RijndaelManaged aes; // for .NET 2 compatibility.

			public void reset()
			{
				 if (rsaProvider != null)
						rsaProvider.Clear();
				 rsaProvider = null;
				 if (aes != null)
						aes.Clear();
				 aes = null;
			}


			public void clearKeys()
			{
				 if (aes != null)
						aes.Clear();
				 aes = null;
			}


			public byte getAsymmetricKeyType()
			{
				 return (byte)wgssSTU.asymmetricKeyType.AsymmetricKeyType_RSA2048;
			}


			public byte getAsymmetricPaddingType()
			{
				 return (byte)wgssSTU.asymmetricPaddingType.AsymmetricPaddingType_OAEP;
			}


			public byte getSymmetricKeyType()
			{
				 return (byte)wgssSTU.symmetricKeyType.SymmetricKeyType_AES256;
			}

			private void create()
			{
				 rsaProvider = new RSACryptoServiceProvider(2048, new CspParameters());
				 rsaParameters = rsaProvider.ExportParameters(false);
			}

			public Array generatePublicKey()
			{
				 if (rsaProvider == null)
						create();
				 return rsaParameters.Modulus;
			}


			public Array getPublicExponent()
			{
				 if (rsaProvider == null)
						create();
				 return rsaParameters.Exponent;
			}


			public void computeSessionKey(Array data)
			{
				 byte[] arr = rsaProvider.Decrypt((byte[])data, true);
				 byte[] key = new byte[32];
				 Array.Copy(arr, arr.Length - 32, key, 0, 32);

				 aes = new RijndaelManaged();
				 aes.Key = key;
				 aes.IV = new byte[16];
				 aes.Mode = CipherMode.CBC;
				 aes.Padding = PaddingMode.None;
			}


			public Array decrypt(Array data)
			{
				 var dec = aes.CreateDecryptor();
				 byte[] arr = new byte[data.Length];
				 dec.TransformBlock((byte[])data, 0, data.Length, arr, 0);
				 return arr;
			}
	 }
}