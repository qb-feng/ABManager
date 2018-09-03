using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

//引入ABSystem插件的AssetBundleDataReader类，进行dep文件的解析
using Tangzx.ABSystem;

namespace HCFeng.ABManage.ABDownLoad
{
    public class AssetBundleDepFileManager : MonoSingleton<AssetBundleDepFileManager>
    {
        #region 私有数据
        //dep.all 文件读写类
        private AssetBundleDataReader curentDepFileReader = null;
        private Dictionary<string, AssetBundleData> curentDepFileABDataDic = null;//dep.all 读取出来的数据
        #endregion

        #region 公有方法
        public void GetUpdateDownLoadDepFile(string serveDepUrl, Action onComplete)
        {
            //下载服务器dep.all文件
            Util.DownLoadFileText(serveDepUrl, (serveDepFileText) =>
            {
                //比较dep文件，形成dep链表
                CompareDepFile(serveDepFileText);
                DebugManager.Log("dep 文件解析完毕！");
                onComplete();
            }, this);
        }
        /// <summary>
        /// 根据ab 的路径名字 获取ab资源信息  参数如：Assets\Prefabs\Capsule.prefab
        /// </summary>
        public AssetBundleData GetAssetBundleDataOfABFullName(string abFullName)
        {
            if (curentDepFileABDataDic != null)
            {
                AssetBundleData tempData;
                if (curentDepFileABDataDic.TryGetValue(abFullName, out tempData))
                {
                    return tempData;
                }
            }
            return null;
        }

        /// <summary>
        /// 解析dep文件  参数1：dep文件路径   返回所有资源名字
        /// </summary>
        public static List<string> ResolveDepFile(string depFilePath)
        {
            var tempReader = new AssetBundleDataReader();
            using (FileStream fs = new FileStream(depFilePath, FileMode.Open, FileAccess.Read))
            {
                tempReader.Read(fs);
                DebugManager.Log(depFilePath + "  dep文件读取成功！读取到的ab资源个数为：" + tempReader.infoMap.Count);
                List<string> resolveDepName = new List<string>();
                foreach (var v in tempReader.infoMap)
                {
                    if (!resolveDepName.Contains(v.Value.fullName))
                    {
                        resolveDepName.Add(v.Value.fullName);
                    }
                }
                tempReader = null;
                return resolveDepName;
            }
        }

        #endregion

        #region MonBehaviour 方法
        protected override void OnDestroy()
        {
            curentDepFileReader = null;
            curentDepFileABDataDic = null;
            base.OnDestroy();
        }
        #endregion

        #region dep.all文件相关
        /// <summary>
        /// 比较本地ab的dep.all文件的文本与服务器的文本得出差异 
        /// 参数1：服务器dep.all文本
        /// </summary>
        private void CompareDepFile(string serveDepFileText)
        {
            string depFilePath = ABDataConfig.localABFullName + ABDataConfig.localDepFileName;

            //找到本地dep文件
            if (File.Exists(depFilePath))
            {
                string value = File.ReadAllText(depFilePath);
                if (serveDepFileText.Equals(value))
                {
                    DebugManager.Log("服务器dep文件与本地dep文件一致！不用更新！");
                    InitDepList(depFilePath);
                    return;
                }
            }
            DebugManager.Log("服务器dep文件与本地dep文件不一致！更新替换！");
            //TODO 此时应当加密写入（暂时不写了）
            FileManager.CreateFile(depFilePath, System.Text.Encoding.UTF8.GetBytes(serveDepFileText));
            InitDepList(depFilePath);
        }

        /// <summary>
        /// 初始化当前最新的dep.all ab列表
        /// </summary>
        private void InitDepList(string depFilePath)
        {
            curentDepFileReader = new AssetBundleDataReader();
            using (FileStream fs = new FileStream(depFilePath, FileMode.Open, FileAccess.Read))
            {
                curentDepFileReader.Read(fs);
                DebugManager.Log(depFilePath + "  dep文件读取成功！读取到的ab资源个数为：" + curentDepFileReader.infoMap.Count);
                curentDepFileABDataDic = new Dictionary<string, AssetBundleData>();
                foreach (var v in curentDepFileReader.infoMap)
                {
                    if (!curentDepFileABDataDic.ContainsKey(v.Value.debugName))
                    {
                        curentDepFileABDataDic.Add(v.Value.debugName, v.Value);
                    }
                    else
                    {
                        DebugManager.LogWarning("dep文件读取到的" + v.Value.debugName + "资源重复！");
                    }
                }
            }
        }
        #endregion
    }
}
