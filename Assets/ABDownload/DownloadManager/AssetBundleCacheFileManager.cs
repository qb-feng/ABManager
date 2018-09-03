using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

//引入absystem插件，使用它打包用的AssetBundleDataReader dep.all 文件读写系统
using Tangzx.ABSystem;

namespace HCFeng.ABManage.ABDownLoad
{
    /// <summary>
    /// ab的cache.txt 文件的管理器
    /// </summary>
    public class AssetBundleCacheFileManager : MonoSingleton<AssetBundleCacheFileManager>
    {
        #region 数据变量
        /// <summary>
        /// 准备下载的ab资源
        /// </summary>
        private List<AssetBundleDownloadModel> waitDownloadAssetBundleList;
        private string serverCacheFileText = null;//服务器cache.txt返回的string
        #endregion

        #region 公有变量
        public delegate void OnGetUpdateABCompleteHandel(List<AssetBundleDownloadModel> result);


        #endregion

        #region MonBehaviour 方法
        protected override void OnDestroy()
        {
            waitDownloadAssetBundleList = null;
            serverCacheFileText = null;
            base.OnDestroy();
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 获取需要下载的ab资源
        /// </summary>
        public void GetUpdateDownLoadAssetBundles(OnGetUpdateABCompleteHandel onComplete)
        {
            string serveDepUrl = ABDataConfig.GetAssetBundleUrl + ABDataConfig.abDepFileApiName;//服务器cache文件路径
            //更新dep文件
            AssetBundleDepFileManager.Instance.GetUpdateDownLoadDepFile(serveDepUrl, () =>
            {
                //比较catch文件
                CompareCacheFileGetDifferentAssetBundles(onComplete);
            });
        }

        /// <summary>
        /// 保存cache文件到沙盒路径在ab文件下载结束后
        /// </summary>
        public void SaveCacheFileOfAssetBundleDownloadOk()
        {
            if (!string.IsNullOrEmpty(serverCacheFileText))
            {
                string cacheFilePath = ABDataConfig.localABFullName + ABDataConfig.localCacheFileName;
                FileManager.CreateFile(cacheFilePath, System.Text.Encoding.UTF8.GetBytes(serverCacheFileText));
                DebugManager.LogWarning(cacheFilePath + "文件保存成功！");
                cacheFilePath = null;
            }
        }

        #endregion

        #region 私有方法

        #region cache.txt文件相关
        /// <summary>
        /// 比较cache.txt文件获取差异ab资源
        /// </summary>
        public void CompareCacheFileGetDifferentAssetBundles(OnGetUpdateABCompleteHandel onComplete)
        {
            string cacheTxtFilePath = ABDataConfig.localABFullName + ABDataConfig.localCacheFileName;//沙盒cache文本路径
            string cacheFileTxt = null;
            if (File.Exists(cacheTxtFilePath))
            {
                DebugManager.LogWarning(cacheTxtFilePath + "沙盒cache文本存在！开始对比差异！");
                //cacheTxtFilePath = Application.dataPath + ABDataConfig.localCacheFilePath;//本地cache文本路径
                //TODO 沙盒下的应该解密读取
                cacheFileTxt = File.ReadAllText(cacheTxtFilePath);
            }
            else
            {
                DebugManager.LogWarning(cacheTxtFilePath + "沙盒cache文本文件不存在！默认将服务器cache文本当做所有资源列表！");
            }

            string serverCacheUrl = ABDataConfig.GetAssetBundleUrl + ABDataConfig.abCacheFileApiName;//服务器cache文件路径

            //下载服务器cache文件
            Util.DownLoadFileText(serverCacheUrl, (serverCacheString) =>
            {
                serverCacheFileText = serverCacheString;//保存服务器cache文件

                GetUpdateDownLoadAssetBundles(cacheFileTxt, serverCacheString);

                onComplete(waitDownloadAssetBundleList);
            }, this);
        }

        /// <summary>
        /// 比较本地ab的cache.txt文件的文本与服务器的文本得出差异 
        /// 参数1：本地cache.txt文本  参数2：服务器cache.txt 文本
        /// </summary>
        private void GetUpdateDownLoadAssetBundles(string localCacheFileText, string serverCacheFileText)
        {
            if (string.IsNullOrEmpty(serverCacheFileText))
            {
                DebugManager.LogError("服务器 cache.txt 的文本存在空值！serverCacheFileText:" + serverCacheFileText);
                return;
            }
            if (serverCacheFileText.Equals(localCacheFileText))
            {
                DebugManager.LogWarning("cache.txt 本地与服务器文本相同,不做比较，继续更新！  localCacheFileText:" + localCacheFileText + "serverCacheFileText:" + serverCacheFileText);
                //return;
            }

            var serverCacheFileData = ResolveCacheFile(serverCacheFileText);//服务器cache.txt数据
            //Dictionary<string, CacheFileDataModel> localCacheFileData = null;
            //if (localCacheFileText == null)
            //{
            //    localCacheFileData = new Dictionary<string, CacheFileDataModel>();
            //}
            //else
            //{
            //    localCacheFileData = ResolveCacheFile(localCacheFileText);//本地cache.txt数据
            //}

            if (waitDownloadAssetBundleList == null)
                waitDownloadAssetBundleList = new List<AssetBundleDownloadModel>();

            //对比服务器与本地的数据
            foreach (var v in serverCacheFileData)
            {
                var tempABData = AssetBundleDepFileManager.Instance.GetAssetBundleDataOfABFullName(v.Key);//dep.ll 文件中的ab信息

                var tempFileUrl = ABDataConfig.localABFullName + tempABData.fullName;//源资源文件路径
                //得到ab文件与ab依赖文件的哈希值组合
                var tempFileHash = HashUtil.GetFileHash(tempFileUrl) + "|" + HashUtil.GetFileHash(tempFileUrl + ABDataConfig.assetBundleManifestFilePostfixName);
                if (v.Value.fileHash.Equals(tempFileHash))
                {
                    DebugManager.Log(tempABData.fullName + "  " + v.Key + "  资源文件与依赖文件哈希值与服务器一致！不用更新！");
                    continue;
                }
                DebugManager.Log(tempABData.fullName + "  " + v.Key + "  需要更新！添加到更新列表！");
                waitDownloadAssetBundleList.Add(new AssetBundleDownloadModel
                {
                    assetBundleFileName = tempABData.fullName,
                    assetBundleManifestFileName = tempABData.fullName + ABDataConfig.assetBundleManifestFilePostfixName,
                    assetBundleName = tempABData.debugName,
                });
            }
        }

        /// <summary>
        /// 解析cache.txt 文本  string ab源资源名字（Assets/perfabs/cube.perfab），value为该ab的哈希值等其他详细信息
        /// </summary>
        private Dictionary<string, CacheFileDataModel> ResolveCacheFile(string cacheFileText)
        {
            try
            {
                Dictionary<string, CacheFileDataModel> cacheFileDataModelDic = new Dictionary<string, CacheFileDataModel>();
                using (StringReader sr = new StringReader(cacheFileText))
                {
                    //TODO 版本比较(暂时不作处理)
                    string vString = sr.ReadLine();

                    //读取缓存ab的信息
                    while (true)
                    {
                        string path = sr.ReadLine();//ab资源的源路径（打包前的路径）
                        if (path == null)
                            break;

                        CacheFileDataModel cache = new CacheFileDataModel();
                        cache.fileHash = sr.ReadLine();
                        cache.metaHash = sr.ReadLine();
                        cache.bundleCrc = sr.ReadLine();
                        int depsCount = Convert.ToInt32(sr.ReadLine());
                        cache.depNames = new string[depsCount];
                        for (int i = 0; i < depsCount; i++)
                        {
                            cache.depNames[i] = sr.ReadLine();
                        }
                        cacheFileDataModelDic.Add(path, cache);
                    }
                }
                return cacheFileDataModelDic;
            }
            catch (Exception e)
            {
                DebugManager.LogError("解析cache.txt 文本 出错！" + e.Message);
            }
            return null;
        }
        #endregion

        #endregion
    }
}