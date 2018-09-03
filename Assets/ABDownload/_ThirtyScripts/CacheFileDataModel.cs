using UnityEngine;
using System.Collections;
namespace HCFeng.ABManage.ABDownLoad
{
    /// <summary>
    /// ab资源的 cache.txt 的数据model    注意：保持与第三方插件absystem中的 Tangzx.ABSystem.AssetCacheInfo 类一致 ！！！！！！！！
    /// </summary>
    public class CacheFileDataModel
    {
        /// <summary>
        /// 源文件的hash，比较变化 此时为了方便下载（改成了fileHash+"|"+mainfestFileHash）
        /// </summary>
        public string fileHash;
        /// <summary>
        /// 源文件meta文件的hash，部分类型的素材需要结合这个来判断变化
        /// 如：Texture
        /// </summary>
        public string metaHash;
        /// <summary>
        /// 上次打好的AB的CRC值，用于增量判断
        /// </summary>
        public string bundleCrc;
        /// <summary>
        /// 所依赖的那些文件
        /// </summary>
        public string[] depNames;
    }
}