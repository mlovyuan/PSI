using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PSI.Common.Encrypt
{
    /// <summary>
    /// 不可逆的加密，用於英文和數字
    /// </summary>
    public class MD5Encrypt
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source">待加密字串</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Encrypt(string source, int length = 32)
        {
            HashAlgorithm provider = CryptoConfig.CreateFromName("MD5") as HashAlgorithm;
            if (string.IsNullOrEmpty(source)) return string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(source);
            byte[] hasValue = provider.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            switch (length)
            {
                case 16:
                    for (int i = 4; i < 12; i++)
                        sb.Append(hasValue[i].ToString("x2"));
                    break;
                case 32:
                    for (int i = 0; i < 16; i++)
                        sb.Append(hasValue[i].ToString("x2"));
                    break;
                default:
                    for (int i = 0; i < hasValue.Length; i++)
                        sb.Append(hasValue[i].ToString("x2"));
                    break;
            }
            return sb.ToString();
        }
    }
}
