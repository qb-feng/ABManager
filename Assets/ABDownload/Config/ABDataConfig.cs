using UnityEngine;
using System.Collections;

namespace HCFeng.ABManage.ABDownLoad
{
    public class ABDataConfig
    {
        #region 本地资源路径
        /// <summary>
        /// 沙盒dep文件名
        /// </summary>
        public static string localDepFileName = "dep.all";
        /// <summary>
        /// 沙盒cache文件名
        /// </summary>
        public static string localCacheFileName = "cache.txt";
        /// <summary>
        /// 下载的ab在本地沙河路径中存放的文件夹名字
        /// </summary>
        public static string localABFullName
        {
            get
            {
                return Application.persistentDataPath + "/AssetBundles/";
            }
        }
        /// <summary>
        /// 安卓包里的AssetBundle文件夹的绝对路径
        /// </summary>
        public static string AppAssetBundleDirName { get { return Util.GetAppStreamingAssetsPath() + "AssetBundles/"; } }

        #endregion

        #region 服务器资源接口变量
        private static string assetBundleUrl = null;
        /// <summary>
        /// 服务器地址
        /// </summary>
        public static string GetAssetBundleUrl
        {
            get
            {
                if (assetBundleUrl == null)
                {
#if UNITY_EDITOR
                    assetBundleUrl = "http://localhost:915/";
#else
                    assetBundleUrl = "http://111.231.206.145:915/";
#endif
                    DebugManager.Log("服务器地址：" + assetBundleUrl);
                }
                return assetBundleUrl;
            }
        }

        /// <summary>
        /// 获取ab cache文件的接口名字
        /// </summary>
        public static string abCacheFileApiName = "api/assetbundle/GetCacheFile";

        /// <summary>
        /// 获取ab dep文件的接口名字
        /// </summary>
        public static string abDepFileApiName = "api/assetbundle/GetDepFile";

        /// <summary>
        /// 获取ab资源的接口名字
        /// </summary>
        public static string getAssetBundleApiName = "api/assetbundle/GetAssetBundle";
        #endregion

        #region  常用变量
        /// <summary>
        /// 资源依赖文件的名字后缀名称（3a85b78f45e66d3862c78b03de9e64e228bedc8c.ab.manifest）
        /// </summary>
        public static string assetBundleManifestFilePostfixName = ".manifest";
        #endregion
    }
}