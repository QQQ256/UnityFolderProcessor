# FolderProcessor

## Introduction

FolderKit是一个基于目录树的工具类，用于读取指定目录下的所有文件夹，并转化为可以获取的文件夹节点；在生成文件夹节点的过程中，默认会读取一些支持的文件类型，例如现在支持读取单个文本文档中的文字并转化为List<string>、图片读取至内存（Texture2D + 图片名称）、以及视频的绝对路径。（我对以上内容的读写是满足自己的需求，如果需要新增数据，可以自行编写新的代码。）

这个工具最主要的价值是把文件夹转换为目录树，并且支持一些快捷的文件夹数据搜索和读取，方便对数据进行获取。



## 视频参考

详情见目录下的bandicam开头视频。

<video src="C:\Users\wxq\PlayDev\GitHub\UnityFolderProcessor\bandicam 2025-10-30 17-14-41-238.mp4" controls=""></video>

## 入口

Boot场景会默认加载StreamingAssets下的Resources文件夹，将内部的所有文件夹和数据进行读取，当图片加载完成后，触发回调跳转场景至SampleScene。

## 示例

SampleScene中基于滚动列表展示了三个例子，可通过滑动查看示意。

![image.png](image.png)

## 默认文件夹结构

默认的文件夹内包含所有需要的数据：图片、文本、视频

![image.png](image%201.png)

## 用户指定文件夹结构

![image.png](image%202.png)

### 例子1

展示了如何通过文件夹名称获取默认文件夹中具体的节点

```cpp
private void UseCase1()
{
    FolderNode folderNodeNameC = FolderProcessDriver.Instance.
	    GetFolderNodeByFolderProcessName("FolderC");
    if (folderNodeNameC != null)
    {
        useCase1.Setup(folderNodeNameC);
    }
}
```

### 例子2

展示了默认文件夹中的所有数据，可以通过点击对应的文件夹节点查看对应数据的内容

```cpp
private void UseCase2()
{
    FolderNode rootFolderNode = FolderProcessDriver.Instance.
	    GetFolderNodeByFolderProcessName("Data");
    if (rootFolderNode != null)
    {
        useCase2.Setup(rootFolderNode);
    }
}
```

### 例子3

展示了如何自定义读取某个路径下的文件夹，并通过类获取根节点进行数据展示。

该例子的使用比较典型，它需要一些参数：

1. 需要指定一个名称用于识别一个FolderProcess
2. 需要指定FolderProcess的RootNode的名称
3. 需要指定读取路径，它一般是绝对路径

```cpp
string folderProcessName = "Case3FolderProcess";
string folderNodeRootName = "Case3Folder";
string folderProcessPath = Path.Combine(Application.dataPath, folderNodeRootName);
```

这些参数都有了之后，调用Driver的CreateFolderProcess函数

```cpp
FolderProcessDriver.Instance.CreateFolderProcess(folderProcessName, 
  // 加载新的文件夹处理流程，指定要读取的文件夹路径
  new InitializeParameters()
{
    LoadPath = folderProcessPath
}, // 初始化完成后的回调，参考Case2的逻辑
    () =>
{
    // 通过自己设定的FolderProcess名称获取对应的RootFolderNode
    FolderNode rootFolderNode = FolderProcessDriver.Instance.
        GetFolderNodeByFolderProcessName(folderProcessName, folderProcessName);

    if (rootFolderNode != null)
    {
        Setup(rootFolderNode);
        
        verticalRect.gameObject.SetActive(true);
        loadButton.gameObject.SetActive(false);
    }
});
```

最后完成目标文件夹的遍历和读取后，回调的使用一般用于进入下个阶段

这里我选择获取刚生成的FolderNode，它需要最初指定的folderProcessName和需要查询的FolderNode Name去查询得到，因此就有以下代码

```cpp
FolderNode rootFolderNode = FolderProcessDriver.Instance.
        GetFolderNodeByFolderProcessName(folderProcessName, folderProcessName);
```

默认的查询则可参考Case1，只需要传入查询的文件夹名称就可以查询默认的FolderProcess中的节点。

也可以通过FolderProcessName + FolderNode Name组合查询对应的内容，例如：

```cpp
// 查询这个 FolderProcess 下的某个特定 FolderNode
FolderNode folderNodeNameC = FolderProcessDriver.Instance.
    GetFolderNodeByFolderProcessName("FolderC", folderProcessName);
if (folderNodeNameC != null)
{
    Debug.Log("Successfully get FolderNode 'FolderC' from FolderProcess '" + folderProcessName + "': " + folderNodeNameC.ToString());
}
```

查询结果：

![image.png](image%203.png)