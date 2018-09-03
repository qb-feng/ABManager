using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace HCFeng.ABManage.ABDownLoad
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Util
    {
        #region 路径相关
        /// <summary>
        /// 获取app内部的StreamingAssets文件夹的绝对路径
        /// </summary>
        public static string GetAppStreamingAssetsPath()
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                default:
                    path = Application.dataPath + "/StreamingAssets/";
                    break;
            }
            return path;
        }
        #endregion

        #region 文件相关

        /// <summary>
        /// 拷贝一个文件夹到另一个文件夹  参数1：源文件夹  参数2：目标文件夹
        /// </summary>
        public static void CopyDirectoryToOtherDir(string srcPath, string destPath)
        {
            if (!Directory.Exists(srcPath))
            {
                DebugManager.LogError("源文件夹路径不存在！" + srcPath);
                return;
            }
            if (!Directory.Exists(destPath))
            {
                DebugManager.LogError("目标文件夹路径不存在！" + destPath);
                return;
            }

            CopyDirectory(srcPath, destPath);
        }

        private static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                DebugManager.LogError("拷贝文件夹失败！" + e.Message);
            }
        }
        #endregion

        #region 资源文件下载相关
        /// <summary>
        /// 下载指定路径文本文件   参数1：文件下载路径   回调参数：下载好的text  参数3：游戏物体
        /// </summary>
        public static void DownLoadFileText(string url, Action<string> onComplete, MonoBehaviour go)
        {
            go.StartCoroutine(DownLoadFileText(url, onComplete));
        }

        /// <summary>
        /// 下载文本文件   参数1：文件下载路径   回调参数：下载好的text
        /// </summary>
        private static IEnumerator DownLoadFileText(string url, Action<string> onComplete)
        {
            using (WWW www = new WWW(url))
            {
                yield return www;
                if (www.isDone)
                {
                    if (string.IsNullOrEmpty(www.error))
                    {
                        onComplete(www.text);
                    }
                    else
                    {
                        DebugManager.LogError("文本文件下载出错！error:" + www.error + "      url:" + url);
                    }
                }
                else
                {
                    DebugManager.LogError("文本文件下载失败 ！url:" + url);
                }

            }
        }
        #endregion
    }
}