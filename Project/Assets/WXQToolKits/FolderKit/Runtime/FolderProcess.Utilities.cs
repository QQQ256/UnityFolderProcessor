using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace WXQToolKits.FolderKit.Runtime
{
    public partial class FolderProcess
    {
        #region 文件夹相关函数

        #region 外部创建节点并存入字典函数
        /// <summary>
        /// 新建一个folderNode
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public FolderNode CreateNewFolderNode(string folderName, FolderNode parentNode = null)
        {
            if (FilesDictionary.TryGetValue(folderName, out FolderNode node))
            {
                return node;
            }

            string absPath = Path.Combine(Application.streamingAssetsPath, folderName);
            // 创建本地文件夹
            if (!Directory.Exists(absPath))
            {
                Directory.CreateDirectory(absPath);
            }

            var newNode = new FolderNode(folderName, parentNode);
            FilesDictionary[folderName] = newNode;
            return newNode;
        }
        #endregion


        private void SortByName(DirectoryInfo[] directoryInfos)
        {
            Array.Sort(directoryInfos, delegate (DirectoryInfo x, DirectoryInfo y)
            {
                // 抓取第一串数字进行比较
                var regex = new Regex(@"\d+");
                int xNumber, yNumber;

                // 尝试从文件夹名中抓取数字
                var xMatch = regex.Match(x.Name);
                var yMatch = regex.Match(y.Name);

                // 如果两个目录名都含数字，则比较数字
                if (xMatch.Success && yMatch.Success)
                {
                    xNumber = int.Parse(xMatch.Value);
                    yNumber = int.Parse(yMatch.Value);
                    return xNumber.CompareTo(yNumber);
                }

                // 如果一个含数字，另一个不含，则数字的目录排在前
                if (xMatch.Success) return -1;
                if (yMatch.Success) return 1;

                // 如果两个都不含数字，则按字典顺序比较
                return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
            });
        }
        

        private static List<FolderNode> CollectLowestLevelFolders(FolderNode node)
        {
            List<FolderNode> result = new List<FolderNode>();

            // Check if current node has no children
            if (node.Children.Count == 0)
            {
                result.Add(node);
            }
            else
            {
                // Recursively search in child nodes
                foreach (FolderNode child in node.Children)
                {
                    result.AddRange(CollectLowestLevelFolders(child));
                }
            }

            return result;
        }
        #endregion


        #region Delete folderNode with folder name

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

        /// <summary>
        /// 给定根目录文件夹，删除内部所有节点
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public bool DeleteFolderNodeWithName(string folderName)
        {
            if (FilesDictionary.TryGetValue(folderName, out FolderNode node))
            {
                RecursiveDeleteFolderNodes(node);

                if (node.Parent != null)
                {
                    node.Parent.Children.Remove(node);
                }

                FilesDictionary.Remove(folderName);

                if (!FilesDictionary.ContainsKey(folderName))
                {
                    Debug.Log($"成功删除了节点{folderName}");
                    return true;
                }
            }

            return false;
        }

        private static void RecursiveDeleteFolderNodes(FolderNode node)
        {
            if (node.Children == null || node.Children.Count == 0)
            {
                return;
            }

            foreach (var child in node.Children.ToList())
            {
                RecursiveDeleteFolderNodes(child);
                FilesDictionary.Remove(child.Name);
            }

            node.Children.Clear();
        }
        #endregion
    }
}
