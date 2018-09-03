# ABManager
## 项目简介
    这是在[ABSystem](https://github.com/tangzx/ABSystem)打包工具的基础上增加的一个相配套的资源下载功能
    项目在配有服务器的基础上可以自动下载更新资源，完成资源热更新
### 文件夹介绍
        Assets\ABDownload\ 为ab资源下载代码以及demo所在
        Assets\ABSystem\ 为ABSystem工具的源码，其中有部分修改，以适应下载功能
### 更新流程
        1 解包过程：
            先判断缓存中是否有dep.all 文件与cache.txt 文件
                有：已经解过包了，继续下一步
                没有：第一次安装，没有解包，开启解包过程
        解包过程：
            安卓步骤：
                1 先从包里的streamAssets文件下拷贝dep.all与cache.txt文件到
                缓存中
                2 解析dep.all文件，得到所有ab的名字，依次从streamAssets文
                件下拷贝出来到缓存中，解包完成
            其他平台步骤：
                直接拷贝包里的streamAssets/AssetBundles/文件夹里的东西到
                缓存中，解包完成
        2 下载dep.all 文件更新保存到本地，并且解析得到ab对象详细信息model 列表
        3 下载cache.txt 文件，解析得到每一个要缓存的ab对象，将该对象的文件哈希值与本地文件的哈希值作对比，不同，则添加到更新列表，相同，不添加。
        4 遍历更新列表，调用服务器接口，下载ab文件到本地
        5 下载完成后保存cache文件到本地，卸载ab下载器
        6 更新完毕，进入游戏
