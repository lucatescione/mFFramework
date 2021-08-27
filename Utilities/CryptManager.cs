using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using mFFramework.Conversions;
using mFFramework.Types;

namespace mFFramework.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class CryptManager
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 128;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }


   
        /// <summary>
        /// Decodifica in base64
        /// </summary>
        /// <param name="stringer"></param>
        /// <returns></returns>
        private static string DecodingFromBase64(string stringer)
        {
            if (!stringer.IsVoid())
            {

                try
                {
                    byte[] buffer = Convert.FromBase64String(stringer);
                    return Encoding.UTF8.GetString(buffer);
                }
                catch
                {
                    return stringer;
                }
                
            }

            return null;
        }



        /// <summary>
        /// Codifica in base64
        /// </summary>
        /// <param name="stringer"></param>
        /// <returns></returns>
        private static string CodingToBase64(string stringer)
        {
            if (!stringer.IsVoid())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(stringer);
                return Convert.ToBase64String(buffer);
            }

            return null;

        }



        /// <summary>
        /// Criptazione asimmetrica
        /// </summary>
        /// <param name="stringer">Stringa da criptare</param>
        /// <param name="alphabets">Alafabeti di conversione</param>
        /// <returns>Stringa criptata</returns>
        public static string EncryptAsymmetric(string stringer, string[] alphabets)
        {
          
            string result = NumericConversion.ConvertAsymmetric(stringer, Types.Encode.Coding, alphabets);

            return result != stringer ? CodingToBase64(result) : stringer;

        }



        /// <summary>
        /// Decvvvriptazione asimmetrica
        /// </summary>
        /// <param name="stringer">Stringa da decriptare</param>
        /// <param name="alphabets">Alafabeti di conversione</param>
        /// <returns>Stringa decriptata</returns>
        public static string DecryptAsymmetric(string stringer, string[] alphabets)
        {
            string result = DecodingFromBase64(stringer);

            return result != stringer ? NumericConversion.ConvertAsymmetric(result, Types.Encode.Decoding, alphabets) : stringer;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringer"></param>
        /// <param name="alphabets"></param>
        /// <returns></returns>
        public static string HashKeyAsymmetric(string stringer, string[] alphabets)
        {

            List<string> parts = NumericConversion.ConvertAsymmetric(stringer.Trim(), Encode.Coding, alphabets)
                                                  .Split('-')
                                                  .ToList();
            long result = 0;
            parts.ForEach(p =>
            {
                for (int i = 0; i < p.Length; i += 3)
                    result += p.Substring(i, 3).ToLong().Value;
            });

            return result.ToString();

        }
    }



}