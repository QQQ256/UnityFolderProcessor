using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FolderProcessor
{
    /// <summary>
    /// Partial class for FolderNode Creation
    /// </summary>
    public partial class FolderProcess
    {
        private static readonly string[] SupportedImageExtensions = { ".png", ".jpg", ".jpeg", ".JPG", ".PNG" };
        private static readonly string[] SupportedTextExtensions = { ".txt" };
        private static readonly string[] SupportedVideoExtensions = { ".mp4", ".mov" };

        private static List<string> _filteredFolderNameList = new();
        
        public void DefaultLoadAllData(string nodeName, Action action = null)
        {
            if (_initialized == false)
            {
                throw new Exception($"{nameof(FolderProcess)} is not initialized yet.");
            }
            
            string nodeFilePath = Application.streamingAssetsPath + "/Resources";
            
            CustomLoadAllData(nodeName, nodeFilePath, action);
        }

        public void CustomLoadAllData(string rootFolderNodeName, string absPath, Action action = null)
        {
            FolderNodeRoot rootNode = CreateRootFolderNode(rootFolderNodeName);
            if (rootNode != null)
            {
                StartCoroutine(AddChildFoldersCoroutine(rootNode, rootNode.RootNode, absPath, 0, action));
            }
        }

        // public void LoadExtraDataFromFolderPath(string folderName, Action action)
        // {
        //     if (_rootFolderNode == null)
        //     {
        //         throw new NullReferenceException("Root FolderNode is null, please call LoadAllData() function first");
        //     }
        //     
        //     var newNode = CreateNewFolderNode(folderName, _rootFolderNode);
        //     StartCoroutine(AddChildFoldersCoroutine(newNode, Application.streamingAssetsPath + "/" + folderName, 0, action));
        // }
        

        #region Set Filter Folders
        public static void SetFilterFolders(List<string> filteredFolderNameList)
        {
            /*
             * example:
             * FolderProcess.Instance.SetFilterFolders(new List<string>()
                    {
                        "FolderName 1",
                        "FolderName 2"
                    });
             */
            _filteredFolderNameList = filteredFolderNameList;
        }
        #endregion

        private static FolderNodeRoot CreateRootFolderNode(string rootNodeName)
        {
            if (RootFolderNodes.ContainsKey(rootNodeName))
            {
                throw new Exception("FolderNode with the same name already exists in RootFolderNodes dictionary.");
            }
            
            var newNode = new FolderNodeRoot(rootNodeName);
            if (RootFolderNodes.TryAdd(rootNodeName, newNode))
            {
                return newNode;
            }
            
            throw new Exception("Failed to create Root FolderNode.");
        }
        

        private IEnumerator AddChildFoldersCoroutine(FolderNodeRoot folderNodeRoot, FolderNode parentNode, string parentFolderPath, int currentDepth, Action action = null)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(parentFolderPath);
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            int directoriesCount = directories.Count(dir => !_filteredFolderNameList.Contains(FolderNodeUtility.TrimPrefix(dir.Name.Substring(dir.Name.LastIndexOf("\\", StringComparison.Ordinal) + 1))));
            int processedCount = 0;
            
            parentNode.SetDepth(currentDepth);

            FolderNodeUtility.SortFolderByPrefixNumber(directories);

            foreach (var directory in directories)
            {
                string key = FolderNodeUtility.TrimPrefix(directory.FullName.Substring(directory.FullName.LastIndexOf("\\", StringComparison.Ordinal) + 1));

                if (_filteredFolderNameList != null && _filteredFolderNameList.Contains(key))
                {
                    Debug.Log($"Skipping folder due to filter: {key}");
                    continue;
                }

                FolderNode folderNode = new FolderNode(key, parentNode);

                folderNodeRoot.AddFolderNodeToDictionary(key, folderNode);

                yield return StartCoroutine(ProcessFilesInDirectory(directory.FullName, folderNode));

                parentNode.Children.Add(folderNode);

                yield return StartCoroutine(AddChildFoldersCoroutine(folderNodeRoot, folderNode, directory.FullName, currentDepth + 1));

                processedCount++;

                parentNode.SetHeight(Math.Max(parentNode.Height, folderNode.Height + 1));

                if (action != null && processedCount == directoriesCount)
                {
                    Debug.Log($"RootFolderNode <color=red>{folderNodeRoot.RootNodeName}</color> created, " +
                              $"folder path is <color=blue>{parentFolderPath}</color>, start BFS load images.");
                    StartCoroutine(BfsLoadAllImages(folderNodeRoot.RootNodeName, parentNode, action));
                }
            }
        }

        private IEnumerator ProcessFilesInDirectory(string directoryPath, FolderNode folderNode)
        {
            string[] files = Directory.GetFiles(directoryPath);

            foreach (var filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (SupportedTextExtensions.Contains(extension))
                {
                    string[] lines = File.ReadAllLines(filePath);

                    folderNode.InitTextInfo(new List<string>(lines));
                }
                else if (SupportedImageExtensions.Contains(extension))
                {
                    folderNode.ImagePaths.Add(filePath);
                }
                else if (SupportedVideoExtensions.Contains(extension))
                {
                    folderNode.AddVideoFullPath(filePath);
                }
            }
            yield return null;
        }
        
        private IEnumerator BfsLoadAllImages(string rootNodeName, FolderNode rootNode, Action action)
        {
            Queue<FolderNode> nodeQueue = new Queue<FolderNode>();
            nodeQueue.Enqueue(rootNode);

            while (nodeQueue.Count > 0)
            {
                FolderNode currentNode = nodeQueue.Dequeue();

                // 加载当前节点的所有图片
                if (currentNode.ImagePaths != null)
                {
                    //Debug.Log("start play with node" + currentNode.Name);
                    foreach (string imagePath in currentNode.ImagePaths)
                    {
                        Texture2D texture = null;
                        yield return StartCoroutine(FolderNodeUtility.LoadImageAsync(imagePath, (loadedTexture) => texture = loadedTexture));

                        if (texture != null)
                        {
                            string textureName = Path.GetFileNameWithoutExtension(imagePath);
                            
                            currentNode.AddImageTexture(new ImageTexture { TextureName = textureName, Texture = texture });
                            texture = null;
                            GC.Collect();
                        }
                    }

                    if (_initializeParameters.ClearImagePathOnLoad)
                    {
                        // 清空路径列表，避免重复加载
                        currentNode.ClearImagePaths();
                    }
                }

                // 将当前节点的所有子节点加入队列中，确保子节点也被处理
                foreach (FolderNode child in currentNode.Children)
                {
                    //Debug.Log(currentNode.Name + "的孩子" + child.Name + "加入队列");
                    //Debug.Log(child.imagePaths.Count);
                    nodeQueue.Enqueue(child);
                }
            }

            Debug.Log($"<color=red>{rootNodeName}</color> textures loaded.");
            action(); // 执行传入的动作
        }
    }
}
