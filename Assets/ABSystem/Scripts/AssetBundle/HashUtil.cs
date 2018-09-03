using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tangzx.ABSystem
{
    public class HashUtil
    {
        /// <summary>
        /// 将AssetBundleUtils 中的获取文件哈希值的方法复制到此处，作为一个公用方法
        /// </summary>
        public static string GetFileHash(string path)
        {
            string _hexStr = null;
            if (!File.Exists(path))
            {
                _hexStr = "FileNotExists";
            }
            else
            {
                using (FileStream fs = new FileStream(path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read))
                {
                    _hexStr = Get(fs);
                    fs.Close();
                }
            }
            return _hexStr;
        }

        public static string Get(Stream fs)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            byte[] bytes = ha.ComputeHash(fs);
            fs.Close();
            return ToHexString(bytes);
        }

        public static string Get(string s)
        {
            return Get(Encoding.UTF8.GetBytes(s));
        }

        public static string Get(byte[] data)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            byte[] bytes = ha.ComputeHash(data);
            return ToHexString(bytes);
        }

        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString().ToLower();
            }
            return hexString;
        }
    }
}
