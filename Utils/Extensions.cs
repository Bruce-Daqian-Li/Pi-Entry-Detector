
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace EDD
{
    public static class Extensions
    {
        //public static T ParseToEnum<T>(this string enumString) where T : struct, Enum => (T)Enum.Parse(typeof(T), enumString, true);
        public static string ToNormalString(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        public static string ToFileNameString(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd-HH-mm-ss");
        public static string ToDetailedString(this DateTime dateTime) => dateTime.ToNormalString() + ":" + dateTime.Millisecond.ToString("000");


        public static T DeepCloneObject<T>(this ISerializable _object) where T : ISerializable
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _object);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }

        public static string Middle(this string str, string After, string Before)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(str)) return result;
            if (str.Contains(After))
            {
                string s1 = str.Split(After)[1];
                if (Before == null || string.Empty == Before) result = s1;
                else
                {
                    string[] s2 = s1.Split(Before);
                    if (s2 != null) result = s2[0];
                }
            }
            return result;
        }
        public static string[] Split(this string str, string Saperator) => str.Split(Saperator.BuildArray(), StringSplitOptions.None);
        public static List<T[]> BreakDown<T>(this T[] data, int size)
        {
            List<T[]> list = new List<T[]>();
            for (int i = 0; i < data.Length / size; i++)
            {
                T[] r = new T[size];
                Array.Copy(data, i * size, r, 0, size);
                list.Add(r);
            }
            if (data.Length % size != 0)
            {
                T[] r = new T[data.Length % size];
                Array.Copy(data, data.Length - (data.Length % size), r, 0, data.Length % size);
                list.Add(r);
            }
            return list;
        }

        public static T[] MergeArray<T>(this IEnumerable<IEnumerable<T>> arraies)
        {
            List<T> list = new List<T>();
            foreach (T[] item in arraies)
            {
                for (int i = 0; i < item.Length; i++) list.Add(item[i]);
            }
            return list.ToArray();
        }

        public static string ToUnicodeString(this string str)
        {
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int)str[i]).ToString("x"));
                }
            }
            return strResult.ToString();
        }

        public static byte[] ToBytesArray(this int n)
        {
            byte[] b = new byte[4];
            for (int i = 0; i < 4; i++) { b[i] = (byte)(n >> (24 - (i * 8))); }
            return b;
        }

        public static int ToInt32(this byte[] b)
        {
            int mask = 0xff;
            int temp = 0;
            int n = 0;
            for (int i = 0; i < b.Length; i++)
            {
                n <<= 8;
                temp = b[i] & mask;
                n |= temp;
            }
            return n;
        }


        public static T[] BuildArray<T>(this T thing) => (T[])thing.ToIEnumerable();
        public static IEnumerable<T> ToIEnumerable<T>(this T thing) => new T[] { thing };

        public static string Stringify<T>(this T value) => JsonConvert.SerializeObject(value, typeof(T), new JsonSerializerSettings());
        public static string StringifyWithFormat<T>(this T value, int indentation = 4)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringWriter textWriter = new StringWriter();
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = indentation,
                IndentChar = ' '
            };
            serializer.Serialize(jsonWriter, value);
            return textWriter.ToString();
        }
        public static bool ToParsedObject<T>(this string JSON, out T data)
        {
            data = JsonConvert.DeserializeObject<T>(JSON);
            return data != null;
        }
        public static string ToBase64(this string plainText) => plainText.ToBase64(Encoding.UTF8);
        public static string ToBase64(this string plainText, Encoding encoding)
        {
            var plainTextBytes = encoding.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string SHAHashEncrypt<TProvider>(this string strIN) where TProvider : HashAlgorithm, new()
        {
            byte[] bytValue = Encoding.UTF8.GetBytes(strIN);
            HashAlgorithm hashAlgo = new TProvider();
            byte[] retVal = hashAlgo.ComputeHash(bytValue);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string SHA256Encrypt(this string strIN) => strIN.SHAHashEncrypt<SHA256CryptoServiceProvider>();
        public static string SHA384Encrypt(this string strIN) => strIN.SHAHashEncrypt<SHA384CryptoServiceProvider>();
        public static string SHA512Encrypt(this string strIN) => strIN.SHAHashEncrypt<SHA512CryptoServiceProvider>();
    }
}
