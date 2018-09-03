namespace HCFeng.ABManage.ABDownLoad
{
    /// <summary>
    /// 下载的ab资源model
    /// </summary>
    [System.Serializable]
    public class AssetBundleDownloadModel
    {
        /// <summary>
        /// 资源名字（资源的唯一标志）(目前名字是该资源在Asset下的路径名字  如Assets\Prefabs\Capsule.prefab)
        /// </summary>
        public string assetBundleName;

        /// <summary>
        /// 资源文件的名字（3a85b78f45e66d3862c78b03de9e64e228bedc8c.ab）
        /// </summary>
        public string assetBundleFileName;

        /// <summary>
        /// 资源依赖文件的名字（3a85b78f45e66d3862c78b03de9e64e228bedc8c.ab.manifest）
        /// </summary>
        public string assetBundleManifestFileName;


        ///// <summary>
        ///// 资源数据的字符串
        ///// </summary>
        //public string assetBundleDataString;

        ///// <summary>
        ///// 资源依赖文件数据的字符串
        ///// </summary>
        //public string assetBundleManifestDataString;

        /// <summary>
        /// 资源数据
        /// </summary>
        public byte[] assetBundleData;

        /// <summary>
        /// 资源依赖文件
        /// </summary>
        public byte[] assetBundleManifestData;

    }
}
