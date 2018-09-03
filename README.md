
## 项目简介
    这是在 https://github.com/tangzx/ABSystem 打包工具的基础上增加的一个相配套的资源下载功能
    项目在配有服务器的基础上可以自动下载更新资源，完成资源热更新
    
### 项目环境
    unity5.4.3f1+ugui
    服务器是基于vs2017+asp.net core 2.1+IIS上搭建的
    
### 文件夹介绍
        Assets\ABDownload\ 为ab资源下载代码以及demo所在
        Assets\ABSystem\ 为ABSystem工具的源码，其中有部分修改，以适应下载功能
            修改部分：
                1 cache.txt 文件放到Assets\StreamingAssets\AssetBundles\文件夹下了
                2 cache.txt 中的fileHash值由原来的ab文件的哈希值改为ab文件的哈希+ab依赖文件的哈希，用来对该文件作对比
                3 菜单栏ABSystem/Builde AssetBundles 改为执行build两次，具体看脚本AssetBundleBuildPanel.cs
                4 其他修改~不记得了。。
                
### 更新流程
        1 解包：
            先判断缓存中是否有dep.all 文件与cache.txt 文件
                有：已经解过包了，跳过1继续下一步
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

### 使用前提
        此工程的资源更新功能是在asp.net core 2.1 服务器的基础上实现的，因此使用前需要有对应的网站，网站服务器代码点 TODO

### 使用步骤
       1 编辑器下测试先加上ABSystem工具要求的宏_AB_MODE_;AB_MODE，再加上新宏EDITOR_ANDROID，
            此宏是用来在编辑器下测试加载AB是从缓存（沙盒路径）中读取
       2 点击菜单栏ABSystem/Build Panel 选择Asset/Prefabs文件夹作为build对象，save后点击Build，build出ab资源
       3 将Assets\StreamingAssets\AssetBundles\下的文件上传到服务器
       4 在ABDataConfig.cs脚本里修改服务器地址及接口名字
       5 运行Assets\ABDownload\GameMain.unity 场景
       
       
       
       
       
       
       
       
