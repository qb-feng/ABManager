using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace HCFeng.ABManage.ABDownLoad
{
    #region 数据结构
    /// <summary>
    /// 下载model
    /// </summary>
    public class DownloadModel
    {
        public byte[] postDatas;
        public byte[] resultDatas;
    }
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// 任务未启动
        /// </summary>
        None = 0,

        /// <summary>
        /// 任务进行中
        /// </summary>
        Tasking = 1,

        /// <summary>
        /// 任务完成
        /// </summary>
        TaskOk = 2,

        /// <summary>
        /// 任务失败
        /// </summary>
        TaskError = 3,
    }

    /// <summary>
    /// 资源下载任务类
    /// </summary>
    public class AssetBundleDownloadTaskModel
    {
        /// <summary>
        /// 任务状态
        /// </summary>
        public TaskState taskState = TaskState.None;

        /// <summary>
        /// 任务描述信息（供界面显示）
        /// </summary>
        public string taskMessage = string.Empty;

        /// <summary>
        /// 还需要下载的ab
        /// </summary>
        public List<AssetBundleDownloadModel> downloadNOModels = new List<AssetBundleDownloadModel>();

        /// <summary>
        /// 下载成功的ab
        /// </summary>
        public List<AssetBundleDownloadModel> downloadOKModels = new List<AssetBundleDownloadModel>();

        /// <summary>
        /// 下载失败的ab
        /// </summary>
        public List<AssetBundleDownloadModel> downloadErrorModels = new List<AssetBundleDownloadModel>();

        private int downLoadModelCount = 0;
        /// <summary>
        /// 当前任务下载的总abmodle 数量
        /// </summary>
        private int DownLoadModelCount
        {
            get
            {
                if (downLoadModelCount == 0)
                {
                    downLoadModelCount = downloadOKModels.Count + downloadNOModels.Count + downloadErrorModels.Count;
                }
                return downLoadModelCount;
            }
        }

        /// <summary>
        /// 下载进度 : 0 - 1
        /// </summary>
        public float Schedule
        {
            get
            {
                if (DownLoadModelCount == 0)
                    return 1;
                return (float)downloadOKModels.Count / DownLoadModelCount;
            }
        }

    }
    #endregion

    /// <summary>
    /// 全局ab下载管理器
    /// </summary>
    public class AssetBundleDownloadManager : MonoSingleton<AssetBundleDownloadManager>
    {

        #region 私有变量
        /// <summary>
        /// 任务下载器状态  true 正在下载 false 空闲中
        /// </summary>
        private bool downloadTaskMangageState = false;


        #region 数据变量
        /// <summary>
        /// 下载完成的ab资源
        /// </summary>
        private List<AssetBundleDownloadModel> okDownloadAssetBundleList;

        /// <summary>
        /// 当前的下载任务队列
        /// </summary>
        private Queue<AssetBundleDownloadTaskModel> downloadTasks = new Queue<AssetBundleDownloadTaskModel>();

        /// <summary>
        /// 当前下载失败的任务队列
        /// </summary>
        private Queue<AssetBundleDownloadTaskModel> downloadErrorTasks = new Queue<AssetBundleDownloadTaskModel>();

        /// <summary>
        /// 当前下载成功的任务栈
        /// </summary>
        private List<AssetBundleDownloadTaskModel> downloadSuccessTasks = new List<AssetBundleDownloadTaskModel>();

        #endregion

        #endregion

        #region MonBehaviour 方法
        protected override void OnDestroy()
        {
            base.OnDestroy();
            okDownloadAssetBundleList = null;
            downloadTasks = null;
            downloadErrorTasks = null;
            downloadSuccessTasks = null;
        }

        /// <summary>
        /// 不断处理下载任务
        /// </summary>
        private void Update()
        {
            if (downloadTasks.Count > 0)
            {
                //移除并返回在 Quene 的顶部的对象
                var tempTask = downloadTasks.Dequeue();
                StarDownloadTaskManage(tempTask);
            }
            if (downloadErrorTasks.Count > 0)
            {
                //TODO 处理下载失败的任务
                //将失败任务添加到下载队列
                var tempTask = downloadErrorTasks.Dequeue();
                downloadTasks.Enqueue(tempTask);
            }
        }

        #endregion

        #region 公有方法
        /// <summary>
        /// 开始更新ab 返回ab任务model
        /// </summary>
        public AssetBundleDownloadTaskModel StarUpdateAssetBundle()
        {
            AssetBundleDownloadTaskModel taskModel = new AssetBundleDownloadTaskModel();
            //检测是否是第一次安装apk，是就先解包
            StartUnPack(taskModel, () =>
            {
                taskModel.taskMessage = "获取更新资源中";
                //获取ab更新资源
                AssetBundleCacheFileManager.Instance.GetUpdateDownLoadAssetBundles((waitDownloadAssetBundleList) =>
                {
                    if (waitDownloadAssetBundleList != null)
                    {
                        taskModel.downloadNOModels = waitDownloadAssetBundleList;
                    }
                    taskModel.taskMessage = "需要更新资源数：" + taskModel.downloadNOModels.Count;
                    //将当前任务压入资源下载栈
                    downloadTasks.Enqueue(taskModel);
                });
            });
            return taskModel;
        }

        /// <summary>
        /// 结束ab资源更新（注意：必须在ab资源全部更新完成后才能调用！）
        /// </summary>
        public void ExitUpdateAssetBundle()
        {
            //保存cache文件
            AssetBundleCacheFileManager.Instance.SaveCacheFileOfAssetBundleDownloadOk();

            //卸载dep manager
            AssetBundleDepFileManager.DestroyInstance();

            //卸载cache mananer
            AssetBundleCacheFileManager.DestroyInstance();

            //卸载abdownload manager
            AssetBundleDownloadManager.DestroyInstance();

            DebugManager.LogWarning("ab下载器卸载成功！");
        }

        #endregion

        #region 私有方法

        #region 单个ab资源部分

        /// <summary>
        /// 启动任务下载器
        /// </summary>
        private void StarDownloadTaskManage(AssetBundleDownloadTaskModel task)
        {

            if (task == null || downloadTaskMangageState || task.downloadNOModels == null)
            {
                return;
            }
            if (task.downloadNOModels.Count == 0)
            {
                task.taskState = TaskState.TaskOk;
                task.taskMessage = "下载完成";
                downloadSuccessTasks.Add(task);
                return;
            }
            try
            {
                task.taskState = TaskState.Tasking;//任务开启中
                task.taskMessage = "开始下载资源";
                StartCoroutine(DownloadTaskMangage(task));
            }
            catch (Exception e)
            {
                task.taskMessage = "下载出错！";
                DebugManager.LogError(task + "任务下载出错！" + e.Message);
                downloadErrorTasks.Enqueue(task);
                downloadTaskMangageState = false;//出错将下载状态设置为false
            }
        }

        /// <summary>
        /// 任务下载器
        /// </summary>
        private IEnumerator DownloadTaskMangage(AssetBundleDownloadTaskModel task)
        {
            downloadTaskMangageState = true;//状态位标志
            string downLoadUrl = ABDataConfig.GetAssetBundleUrl + ABDataConfig.getAssetBundleApiName;
            DebugManager.Log("任务开始下载：" + task.ToString());

            AssetBundleDownloadModel tempABModel = null;//中间变量

            #region 遍历未下载列表资源，开启下载
            for (int i = 0; i < task.downloadNOModels.Count; ++i)
            {
                tempABModel = task.downloadNOModels[i];

                DownloadModel downModel = new DownloadModel();

                //下载资源文件
                task.taskMessage = "下载资源" + tempABModel.assetBundleFileName;
                downModel.postDatas = System.Text.Encoding.UTF8.GetBytes(tempABModel.assetBundleFileName);
                yield return DownloadABFile(downLoadUrl, downModel);
                tempABModel.assetBundleData = downModel.resultDatas;

                //下载资源mainfest文件
                task.taskMessage = "下载资源" + tempABModel.assetBundleManifestFileName;
                downModel.postDatas = System.Text.Encoding.UTF8.GetBytes(tempABModel.assetBundleManifestFileName);
                yield return DownloadABFile(downLoadUrl, downModel);
                tempABModel.assetBundleManifestData = downModel.resultDatas;


                if (tempABModel != null && tempABModel.assetBundleManifestData != null && tempABModel.assetBundleData != null)
                {
                    DebugManager.Log("资源：" + tempABModel.assetBundleName + "下载成功！准备写入本地文件！");
                    //TODO 写入文件
                    bool result = FileManager.CreateFile(ABDataConfig.localABFullName + tempABModel.assetBundleFileName, tempABModel.assetBundleData);
                    if (result)
                    {
                        result = FileManager.CreateFile(ABDataConfig.localABFullName + tempABModel.assetBundleManifestFileName, tempABModel.assetBundleManifestData);
                    }

                    if (result)
                        task.downloadOKModels.Add(tempABModel);//添加到成功队列中
                    else
                    {
                        DebugManager.Log("资源：" + tempABModel.assetBundleName + "下载成功，写入文件异常！");
                        task.downloadErrorModels.Add(tempABModel);//添加到失败队列中
                    }
                }
                else
                {
                    DebugManager.Log("资源：" + tempABModel.assetBundleName + "下载异常！数据错误！");
                    task.downloadErrorModels.Add(tempABModel);//添加到失败队列中
                }

                task.downloadNOModels.RemoveAt(i--);//移出未下载队列
                tempABModel = null;
            }
            #endregion

            //判断任务是否成功或者失败
            if (task.downloadErrorModels != null && task.downloadErrorModels.Count > 0)
            {
                DebugManager.LogWarning("任务：" + task.ToString() + "下载失败！");
                task.taskState = TaskState.TaskError;
                task.taskMessage = task.downloadErrorModels.Count + "个资源下载失败！重新下载！";
                downloadErrorTasks.Enqueue(task);
            }
            else
            {
                DebugManager.LogWarning("任务：" + task.ToString() + "下载成功！");
                task.taskState = TaskState.TaskOk;
                task.taskMessage = "下载完成";
                downloadSuccessTasks.Add(task);
            }

            //当前任务下载结束
            downloadTaskMangageState = false;
        }

        private Dictionary<string, string> abDownLoadHeads = new Dictionary<string, string> { { "Content-Type", "application/octet-stream" } };
        /// <summary>
        /// 下载ab文件的方法  参数1:下载地址 参数2：byte[] 数据
        /// </summary>
        private IEnumerator DownloadABFile(string downLoadUrl, DownloadModel downloadModel)
        {
            using (WWW www = new WWW(downLoadUrl, downloadModel.postDatas, abDownLoadHeads))
            {
                yield return www;
                if (www.isDone && string.IsNullOrEmpty(www.error))
                {
                    downloadModel.resultDatas = www.bytes;
                }
                else
                {
                    DebugManager.LogError(downLoadUrl + "下载出错！error:" + www.error);
                }
            }
        }

        #endregion

        #region 初始化安装解包过程

        /// <summary>
        /// 解包过程 参数1：任务model  参数2：回调
        /// </summary>
        private void StartUnPack(AssetBundleDownloadTaskModel taskModel, Action onComplete)
        {
            taskModel.taskMessage = "检查安装包资源";
            var localAbFullName = ABDataConfig.localABFullName;
            if (!Directory.Exists(localAbFullName))
            {
                taskModel.taskMessage = "开始解包";
                Directory.CreateDirectory(localAbFullName);
                DebugManager.Log(localAbFullName + "文件夹不存在，自动创建！开始解包！");

                if (Application.platform == RuntimePlatform.Android)
                {
                    //安卓环境下必须用www访问StreamingAssets文件夹下的文件
                    StartCoroutine(StartAndroidUnPack(taskModel, onComplete));
                }
                else
                {
                    Util.CopyDirectoryToOtherDir(ABDataConfig.AppAssetBundleDirName, localAbFullName);
                    DebugManager.Log("解包完成！");
                    taskModel.taskMessage = "解包完成";
                    onComplete();
                }
            }
            else
            {
                DebugManager.Log(localAbFullName + "文件夹存在！不需要解包！");
                onComplete();
            }
        }


        /// <summary>
        /// 安卓解包过程
        /// </summary>
        private IEnumerator StartAndroidUnPack(AssetBundleDownloadTaskModel taskModel, Action onComplete)
        {
            string resDicUrl = ABDataConfig.AppAssetBundleDirName;//安卓包资源路径
            string targetDicUrl = ABDataConfig.localABFullName;//目标路径
            string[] copyFile = new string[] 
            {
                ABDataConfig.localCacheFileName,
                ABDataConfig.localDepFileName,
            };
            //先拷贝cache.txt 与dep.file文件
            taskModel.taskMessage = "解压配置文件";
            yield return CopyAssetBundle(resDicUrl, targetDicUrl, copyFile);

            //解析dep文件 得到所有的ab 名字
            var abNames = AssetBundleDepFileManager.ResolveDepFile(targetDicUrl + copyFile[1]);
            //添加mainfest文件
            int count = abNames.Count;
            for (int i = 0; i < count; ++i)
            {
                abNames.Add(abNames[i] + ABDataConfig.assetBundleManifestFilePostfixName);
            }
            //在拷贝所有ab文件
            taskModel.taskMessage = "解压ab文件";
            yield return CopyAssetBundle(resDicUrl, targetDicUrl, abNames);

            DebugManager.Log("解包完成！");
            taskModel.taskMessage = "解包完成";
            yield return 0;
            onComplete();
        }

        /// <summary>
        /// 【从参数1文件夹】中拷贝【参数3中的文件名】到【参数2文件夹】中
        /// </summary>
        private IEnumerator CopyAssetBundle(string resDicUrl, string targetDicUrl, IEnumerable fileNames)
        {
            foreach (var v in fileNames)
            {
                using (WWW www = new WWW(resDicUrl + v))
                {
                    yield return www;
                    if (www.isDone && string.IsNullOrEmpty(www.error))
                    {
                        using (FileStream fs = new FileStream(targetDicUrl + v, FileMode.OpenOrCreate))
                        {
                            fs.Write(www.bytes, 0, www.bytes.Length);
                            fs.Close();
                        }
                    }
                    else
                    {
                        DebugManager.LogError("文件拷贝出错!" + resDicUrl + v + "error:" + www.error);
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}

