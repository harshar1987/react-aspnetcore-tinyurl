using System;
using System.Security.Cryptography;
using System.Text;

namespace UrlShortnerApi.Extensions
{

    public static class StringExtensions
    {
        public static string MD5Hash(this string input)
        {

            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }

            return hash.ToString();
        }

        public static string ToBase64(this string input)
        {
            var toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(input);
            return Convert.ToBase64String(toEncodeAsBytes);
        }
    }
}