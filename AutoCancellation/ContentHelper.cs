using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCancellation
{
    public class ContentHelper
    {
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startStr">开始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns>介于开始和结束字符串之间的字符串</returns>
        public static string GetMidString(string str, string startStr, string endStr)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(startStr) || string.IsNullOrEmpty(endStr))
            {
                return string.Empty;
            }

            int startIndex = str.IndexOf(startStr, StringComparison.CurrentCultureIgnoreCase);

            if (startIndex == -1)
            {
                return string.Empty;
            }

            startIndex += startStr.Length;

            int endIndex = str.IndexOf(endStr, startIndex, StringComparison.CurrentCultureIgnoreCase);

            return str.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// 将Unicode字符串转成汉字
        /// </summary>
        /// <param name="str">包含Unicode字符的字符串</param>
        /// <returns></returns>
        public static string Unicode2Character(string str)
        {
            string[] strItem = str.Split(new string[] { "\\u" }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strItem.Length; i++)
            {
                if (strItem[i].Length < 4)
                {
                    sb.Append(strItem[i]);
                }
                else
                {
                    sb.Append(Unicode2UnitCharacter(strItem[i].Substring(0, 4)));
                    if (strItem[i].Length > 4)
                    {
                        sb.Append(strItem[i].Substring(4));
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 把四个字符长度的Unicode转成对应的汉字
        /// </summary>
        /// <param name="str">长度是4的Unicode</param>
        /// <returns>对应的汉字，若转换出错则返回原字符串</returns>
        static string Unicode2UnitCharacter(string str)
        {
            try
            {
                byte code = System.Convert.ToByte(str.Substring(0, 2), 16);
                byte code2 = System.Convert.ToByte(str.Substring(2), 16);
                return System.Text.Encoding.Unicode.GetString(new byte[] { code2, code });
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static string Escape(string str)
        {
            if (str == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            int len = str.Length;

            for (int i = 0; i < len; i++)
            {
                char c = str[i];

                //everything other than the optionally escaped chars _must_ be escaped
                if (Char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '/' || c == '\\' || c == '.')
                    sb.Append(c);
                else
                    sb.Append(Uri.HexEscape(c));
            }

            return sb.ToString();
        }

        public static string UnEscape(string str)
        {
            if (str == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            int len = str.Length;
            int i = 0;
            while (i != len)
            {
                if (Uri.IsHexEncoding(str, i))
                    sb.Append(Uri.HexUnescape(str, ref i));
                else
                    sb.Append(str[i++]);
            }

            return sb.ToString();
        }
    }
}
