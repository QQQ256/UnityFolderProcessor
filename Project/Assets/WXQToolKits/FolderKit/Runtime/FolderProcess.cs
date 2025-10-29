using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FolderProcessor
{
    [Serializable]
    public class FolderProcess
    {
        [SerializeField]
        private FolderNode rootNode;
        
        private InitializeParameters _initializeParameters;
        private string _folderProcessName;
        private bool _initialized;
        
        private Dictionary<string, FolderNode> _nodeDictionary;
        private List<string> _filteredFolderNameList = new();
        
        private static MonoBehaviour _driverInstance;
        private static readonly string[] SupportedImageExtensions = { ".png", ".jpg", ".jpeg", ".JPG", ".PNG" };
        private static readonly string[] SupportedTextExtensions = { ".txt" };
        private static readonly string[] SupportedVideoExtensions = { ".mp4", ".mov" };

        
        public void LoadAsync(Action loadedAction)
        {
            if (_initialized == false)
                throw new Exception($"{nameof(FolderProcess)} is not initialized yet.");

            if (rootNode == null)
                rootNode = new FolderNode(_folderProcessName);
            else
                throw new Exception("Root FolderNode is already created.");
            
            
            string absPath = _initializeParameters.LoadPath;
            
            AddFolderNodeToDictionary(_folderProcessName, rootNode);
            
            _driverInstance.StartCoroutine(AddChildFoldersCoroutine(rootNode, absPath, 0, loadedAction));
        }
        
        internal void Initialize(InitializeParameters parameters, MonoBehaviour driver, string folderProcessName)
        {
            CheckInitializeParameters(parameters);

            _nodeDictionary = new Dictionary<string, FolderNode>();
            _folderProcessName = folderProcessName;
            _driverInstance = driver;
            _initialized = true;
        }
        
        internal FolderNode GetFolderNodeWithFolderOriginalName(string folderName)
        {
            if (_nodeDictionary.TryGetValue(folderName, out FolderNode targetNode))
            {
                return targetNode;
            }

            throw new Exception($"Failed to get FolderNode: {folderName}");
        }

        private void CheckInitializeParameters(InitializeParameters parameters)
        {
            if (_initialized)
            {
                throw new Exception($"{nameof(FolderProcess)} is initialized yet.");
            }
            
            if (parameters == null)
                throw new Exception($"{nameof(FolderProcess)} create parameters is null.");

            _initializeParameters = parameters;
        }



        #region 加载节点逻辑

        private IEnumerator AddChildFoldersCoroutine(FolderNode parentNode, string parentFolderPath, int currentDepth, Action action = null)
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

                AddFolderNodeToDictionary(key, folderNode);

                yield return _driverInstance.StartCoroutine(ProcessFilesInDirectory(directory.FullName, folderNode));

                parentNode.Children.Add(folderNode);

                yield return _driverInstance.StartCoroutine(AddChildFoldersCoroutine(folderNode, directory.FullName, currentDepth + 1));

                processedCount++;

                parentNode.SetHeight(Math.Max(parentNode.Height, folderNode.Height + 1));

                if (action != null && processedCount == directoriesCount)
                {
                    Debug.Log($"FolderProcess <color=red>{_folderProcessName}</color> created, " +
                              $"folder path is <color=blue>{parentFolderPath}</color>, start BFS load images.");
                    _driverInstance.StartCoroutine(BfsLoadAllImages(parentNode, action));
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
        
        private IEnumerator BfsLoadAllImages(FolderNode startNode, Action action = null)
        {
            Queue<FolderNode> nodeQueue = new Queue<FolderNode>();
            nodeQueue.Enqueue(startNode);

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
                        yield return _driverInstance.StartCoroutine(FolderNodeUtility.LoadImageAsync(imagePath, (loadedTexture) => texture = loadedTexture));

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

            Debug.Log($"<color=red>{_folderProcessName}</color> textures loaded, the final load phase done!");
            action?.Invoke();
        }

        #endregion
        
        
        #region 删除节点逻辑

        internal bool RemoveFolderNode(string rootFolderName, string folderToRemove)
        {
            var nodeDictionary = _nodeDictionary;
            
            if (nodeDictionary.TryGetValue(folderToRemove, out FolderNode node))
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

                nodeDictionary.Remove(folderToRemove);
                if (!nodeDictionary.ContainsKey(folderToRemove))
                {
                    Debug.Log($"Successfully delete folder node{folderToRemove}, which is from the root node {rootFolderName}");
                    return true;
                }
            }
           

            return false;
        }

        private void RecursiveDeleteFolderNodes(FolderNode node)
        {
            if (node.ChildCount == 0)
            {
                return;
            }

            var nodeDictionary = _nodeDictionary;
            foreach (var child in node.Children)
            {
                child.Clear();
                RecursiveDeleteFolderNodes(child);
                nodeDictionary.Remove(child.Name);
            }

            node.Children.Clear();
        }

        #endregion
        
        

        private void AddFolderNodeToDictionary(string key, FolderNode value) => _nodeDictionary[key] = value;

        public void ClearAllFolderNodeData()
        {
            foreach (var folderNode in _nodeDictionary)
            {
                folderNode.Value.Clear();
            }
            _nodeDictionary.Clear();
            _nodeDictionary = null;
            
            Debug.Log("All FolderNode data cleared.");
        }
        
        
        
        


        // #region 创建节点文件夹逻辑，测试中
        //
        // /// <summary>
        // /// 创建节点并存储数据到磁盘
        // /// </summary>
        // /// <param name="nodeName">节点名称（如"TestNode"）</param>
        // /// <param name="absPath">绝对路径（如"D:/MyProject/Resources"）</param>
        // /// <param name="srcImagePath">图片文件路径列表（需全路径），这里选择复制目标文件</param>
        // /// <param name="textLines">文本内容列表（每行一个字符串）</param>
        // public FolderNode CreateFolderNodeInDisk(
        //     string nodeName,
        //     string absPath,
        //     string srcImagePath,
        //     List<string> textLines)
        // {
        //     // 1. 创建文件夹节点
        //     FolderNode newNode = new FolderNode(nodeName);
        //     // 暂时不加入任何字典和parent
        //     // FilesDictionary[nodeName] = newNode;
        //
        //     // 2. 确保目标目录存在
        //     string targetDir = Path.Combine(absPath, nodeName);
        //     if (!Directory.Exists(targetDir))
        //     {
        //         Directory.CreateDirectory(targetDir);
        //         Debug.Log($"创建目录: {targetDir}");
        //     }
        //
        //     // 3. 存储文本到 word.txt
        //     if (textLines is { Count: > 0 })
        //     {
        //         string textFilePath = Path.Combine(targetDir, "word.txt");
        //         File.WriteAllLines(textFilePath, textLines);
        //         newNode.InitTextInfo(textLines);
        //         Debug.Log($"文本已保存到: {textFilePath}");
        //     }
        //
        //     // 4. 复制图片到目标目录
        //     if (!File.Exists(srcImagePath))
        //     {
        //         Debug.LogWarning($"图片不存在: {srcImagePath}");
        //         return null;
        //     }
        //
        //     string destImagePath = Path.Combine(targetDir, Path.GetFileName(srcImagePath));
        //     // File.Copy(srcImagePath, destImagePath, overwrite: true);
        //     // newNode.ImagePaths().Add(destImagePath);
        //     Debug.Log($"图片已复制到: {destImagePath}");
        //
        //     // 5. 递归更新父节点高度（模拟原逻辑）
        //     UpdateParentNodeHeight(newNode);
        //
        //     return newNode;
        // }
        //
        // /// <summary>
        // /// 模拟原逻辑中的父节点高度更新
        // /// </summary>
        // internal void UpdateParentNodeHeight(FolderNode node)
        // {
        //     if (node.Parent != null)
        //     {
        //         node.Parent.SetHeight(Math.Max(node.Parent.Height, node.Height + 1));
        //         UpdateParentNodeHeight(node.Parent);
        //     }
        // }
        //
        // #endregion
    }
}