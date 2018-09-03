using System;
using System.IO;
using System.Collections;

namespace HCFeng.ABManage.ABDownLoad
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public class FileManager
    {

        /// <summary>
        /// 覆盖创建空文件 参数1：文件路径
        /// </summary>
        public static bool CreateFile(string path)
        {
            return CreateFile(path, null, true);
        }

        /// <summary>
        /// 覆盖创建文件 参数1：文件路径 参数2：文件数据 
        /// </summary>
        public static bool CreateFile(string path, byte[] data)
        {
            return CreateFile(path, data, true);
        }

        /// <summary>
        /// 覆盖创建文件 参数1：文件路径 参数2：文件数据 
        /// </summary>
        public static bool CreateFile(string path, string data)
        {
            return CreateFile(path, System.Text.Encoding.UTF8.GetBytes(data), true);
        }


        /// <summary>
        /// 创建文件 参数1：文件路径 参数2：文件数据 参数3：是否覆盖创建
        /// </summary>
        public static bool CreateFile(string path, byte[] data, bool overCreate)
        {
            try
            {
                if (overCreate && File.Exists(path))
                {
                    File.Delete(path);
                }
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    if (data != null)
                    {
                        fs.Write(data, 0, data.Length);
                    }
                    fs.Close();
                }
                DebugManager.Log(path + "文件创建成功！");
                return true;
            }
            catch (Exception e)
            {
                DebugManager.LogError(path + "文件创建失败！error:" + e.Message);
            }

            return false;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                DebugManager.Log(path + "文件删除成功！");
            }
        }

    }
}
