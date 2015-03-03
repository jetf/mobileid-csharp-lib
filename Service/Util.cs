using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace MobileId
{
    /// <summary>
    /// Helper methods used by various parts of Mobile ID client
    /// </summary>
    public static class Util
    {
        public static string CurrentTimeStampString() {
             return string.Format("{0:yyyy-MM-ddTHH:mm:ss.ffffffZ}", System.DateTime.UtcNow);
        }

        public static bool IsXmlSafe(string data)
        {
            return data != null && ! Regex.Match(data, "[<>\"]").Success;
        }

        public static string Str(string s)
        {
            return s != null ? ("\"" + s + "\"") : "null";
        }

        public static UserLanguage ParseUserLanguage(string language)
        {
            if (string.IsNullOrEmpty(language)) throw new ArgumentNullException("ParseUserLanguage");
            string s = language.ToLower();
            switch (s)
            {
                case "en": return UserLanguage.en;
                case "de": return UserLanguage.de;
                case "it": return UserLanguage.it;
                case "fr": return UserLanguage.fr;
                default: throw new ArgumentOutOfRangeException("ParseUserLanguage");
            }
        }

        public static StoreLocation ParseKeyStoreLocation(string s)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException();
            switch (s) {
                case "CurrentUser": return StoreLocation.CurrentUser;
                case "LocalMachine": return StoreLocation.LocalMachine;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static RandomNumberGenerator cprng;

        /// <summary>
        /// return a 32-hexchar random string with 64-128 bit randomness.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static string Build64bitRandomHex(string seed)
        {
            MD5 md5 = MD5.Create();
            byte[] seedBytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(seed)); // 64-bit seed
            byte[] rndBytes = new byte[8]; // 64-bit random
            byte[] buffer = new byte[16];
            if (cprng == null)
                cprng = new RNGCryptoServiceProvider();
            cprng.GetBytes(rndBytes);
            for (int i = 0; i < 8; i++)
                buffer[i] = seedBytes[i];
            for (int i = 8; i < 16; i++)
                buffer[i] = rndBytes[i - 8];
            byte[] hash = md5.ComputeHash(buffer);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }

        //public static ServiceStatusCode ParseStatusCode(string s)
        //{
        //    return ServiceStatusCode.GeneralError; // TODO
        //}
    }
}
