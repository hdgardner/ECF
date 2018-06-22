using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements operations for the encryption manager.
    /// </summary>
    public class EncryptionManager
    {
        /// <summary>
        /// Converts an byte array to a hexadecimal encoded string.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>The hexadecimal encoded bytes.</returns>
        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString.Append(bytes[i].ToString("X2"));
            }
            return hexString.ToString();
        }

        /// <summary>
        /// Converst a byte array to a Base64 encoded string.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>The Base64 encoded bytes.</returns>
        public static string BytesToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Encrypts the specified STR plain text.
        /// </summary>
        /// <param name="strPlainText">The STR plain text.</param>
        /// <returns></returns>
        public static string Encrypt(string strPlainText)
        {
            string strKey16 = ConfigurationManager.AppSettings["Mediachase-EncryptionPrivateKey"];
            TripleDESCryptoServiceProvider crp = new TripleDESCryptoServiceProvider();
            UnicodeEncoding uEncode = new UnicodeEncoding();
            ASCIIEncoding aEncode = new ASCIIEncoding();

            // Store plaintext as a byte array
            byte[] bytPlainText = uEncode.GetBytes(strPlainText);

            // Create a memory stream for holding encrypted text
            MemoryStream stmCipherText = new MemoryStream();

            // Private key
            crp.Key = aEncode.GetBytes(strKey16.Substring(0, 16));

            // Initialization vector is the encryption seed 
            crp.IV = aEncode.GetBytes(strKey16.Substring(8, 8));
            crp.BlockSize = 8;

            // Create a crypto-writer to encrypt a bytearray into a stream
            CryptoStream csEncrypted = new CryptoStream(stmCipherText, crp.CreateEncryptor(), CryptoStreamMode.Write);
            csEncrypted.Write(bytPlainText, 0, bytPlainText.Length);
            csEncrypted.FlushFinalBlock();

            // Return result as a Base64 encoded string
            return Convert.ToBase64String(stmCipherText.ToArray());
        }

        /// <summary>
        /// Decrypts the specified STR cipher text.
        /// </summary>
        /// <param name="strCipherText">The STR cipher text.</param>
        /// <returns></returns>
        public static string Decrypt(string strCipherText)
        {
            string strKey16 = ConfigurationManager.AppSettings["Mediachase-EncryptionPrivateKey"];
            TripleDESCryptoServiceProvider crp = new TripleDESCryptoServiceProvider();
            UnicodeEncoding uEncode = new UnicodeEncoding();
            ASCIIEncoding aEncode = new ASCIIEncoding();

            // Store cipher text as a byte array
            byte[] bytCipherText = Convert.FromBase64String(strCipherText);
            MemoryStream stmPlainText = new MemoryStream();
            MemoryStream stmCipherText = new MemoryStream(bytCipherText);

            // Private key
            crp.Key = aEncode.GetBytes(strKey16.Substring(0, 16));

            // Initialization vector
            crp.IV = aEncode.GetBytes(strKey16.Substring(8, 8));

            // Create a crypto stream decoder to decode a cipher text stream into a plain text stream
            CryptoStream csDecrypted = new CryptoStream(stmCipherText, crp.CreateDecryptor(), CryptoStreamMode.Read);
            StreamWriter sw = new StreamWriter(stmPlainText);
            StreamReader sr = new StreamReader(csDecrypted);
            sw.Write(sr.ReadToEnd());

            // Clean up afterwards
            sw.Flush();
            csDecrypted.Clear();
            crp.Clear();
            return uEncode.GetString(stmPlainText.ToArray());
        }
    }

}
