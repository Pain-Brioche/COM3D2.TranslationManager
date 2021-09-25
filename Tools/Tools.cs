using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using Newtonsoft.Json;

namespace Translation_Manager
{
    public class Tools
    {
        /// <summary>
        /// Create directory helper
        /// </summary>
        /// <param name="folderPath"></param>
        public static void MakeFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// Returns translation type
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static TlTypes GetTlType(string text)
        {
            if (text == "Japanese") { return TlTypes.Japanese; }
            else if (text == "Official") { return TlTypes.Official; }
            else if (text == "DeepL") { return TlTypes.DeepL; }
            else if (text == "Google") { return TlTypes.Google; }
            else { return TlTypes.Invalid; }
        }

        /// <summary>
        /// Returns search Mode
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static TlTypes GetSearchMode(string text)
        {
            if (text == "In Japanese") { return TlTypes.Japanese; }
            else if (text == "In Official") { return TlTypes.Official; }
            else if (text == "In DeepL") { return TlTypes.DeepL; }
            else if (text == "In Google") { return TlTypes.Google; }
            else { return TlTypes.Invalid; }
        }

        /// <summary>
        /// Returns MD5 hashvalue of a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5(string str)
        {
            string hashValue;

            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                hashValue = sb.ToString().ToLower();
            }
            return hashValue;
        }
    }


    public enum TlTypes
    {
        Japanese,
        Official,
        DeepL,
        Google,
        Invalid
    }
}