using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FolderProcessor
{
    public partial class FolderProcess
    {
        // #region 外部创建节点并存入字典函数
        // /// <summary>
        // /// 新建一个folderNode
        // /// </summary>
        // /// <param name="folderName"></param>
        // /// <returns></returns>
        // public FolderNode CreateNewFolderNode(string folderName, FolderNode parentNode = null)
        // {
        //     if (DefaultFolderNode.TryGetValue(folderName, out FolderNode node))
        //     {
        //         return node;
        //     }
        //
        //     string absPath = Path.Combine(Application.streamingAssetsPath, folderName);
        //     // 创建本地文件夹
        //     if (!Directory.Exists(absPath))
        //     {
        //         Directory.CreateDirectory(absPath);
        //     }
        //
        //     var newNode = new FolderNode(folderName, parentNode);
        //     FilesDictionary[folderName] = newNode;
        //     return newNode;
        // }
        // #endregion


        #region 删除文件夹

        /*
         * 使用示例，晋城康养指南，先截图，再删除所需要截图的文件夹数据
         * FolderProcess.Instance.LoadAllData(() =>
                {
                    StartCoroutine(CameraScreenShot.Instance.CollectAllScreenShots("四大关键词图文", () =>
                    {
                        // 将图文文件夹中的数据释放
                        if (FolderProcess.DeleteFolderNodeWithName("四大关键词图文"))
                        {
                            FolderProcess.Instance.LoadDataFromFolders("LabelScreenShot", () =>
                            {
                                Singleton<UIManager>.Create();
                                Singleton<ContextManager>.Create();
                            });
                        }
                    }));
                });
         */

        public static bool RemoveFolderNode(string rootFolderName, string folderToRemove)
        {
            if (RootFolderNodes.TryGetValue(rootFolderName, out FolderNodeRoot nodeRoot))
            {
                var targetFolderDic = nodeRoot.FilesDictionary;
                if (targetFolderDic.TryGetValue(folderToRemove, out FolderNode node))
                {
                    RecursiveDeleteFolderNodes(node);

                    if (node.Parent != null)
                    {
                        var nodeList = node.ParentChildrenNodes;
                        if (nodeList.Contains(node))
                        {
                            node.Clear();
                            nodeList.Remove(node);
                        }
                    }

                    targetFolderDic.Remove(folderToRemove);
                    if (!targetFolderDic.ContainsKey(folderToRemove))
                    {
                        Debug.Log($"Successfully delete folder node{folderToRemove}, which is from the root node {rootFolderName}");
                        return true;
                    }
                }
            }
           

            return false;
        }

        private static void RecursiveDeleteFolderNodes(FolderNode node, string rootFolderName = DefaultRootNodeName)
        {
            if (node.ChildCount == 0)
            {
                return;
            }

            var targetFolderDic = RootFolderNodes[rootFolderName].FilesDictionary;
            foreach (var child in node.Children)
            {
                child.Clear();
                RecursiveDeleteFolderNodes(child);
                targetFolderDic.Remove(child.Name);
            }

            node.Children.Clear();
        }
        #endregion
    }
}
