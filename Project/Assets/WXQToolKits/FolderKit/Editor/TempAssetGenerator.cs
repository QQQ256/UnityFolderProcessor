#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WXQToolKits.FolderKit.Runtime;

namespace WXQToolKits.FolderKit.Editor
{
    public static class TempAssetGenerator
    {
        private const string TempRoot = "Assets/~TempTextures";
        private static readonly Dictionary<string, FolderNode> PathToNodeMap = new Dictionary<string, FolderNode>();
        
        private static readonly List<string> PendingFolders = new List<string>();
        private static readonly Dictionary<string, FolderNode> NodeMap = new Dictionary<string, FolderNode>();

        public static void ProcessScanning(string rootPath, FolderNode rootNode)
        {
            // 清理旧数据
            if (Directory.Exists(TempRoot))
            {
                AssetDatabase.DeleteAsset(TempRoot);
            }
            Directory.CreateDirectory(TempRoot);
            PathToNodeMap.Clear();

            // 开始批量处理
            AssetDatabase.StartAssetEditing();
            try
            {
                string relativePath = "Assets" + rootPath.Substring(Application.dataPath.Length);
                PendingFolders.Add(relativePath);
                AsyncScan(rootNode);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }
        
        private static void AsyncScan(FolderNode nodeRoot)
        {
            const int maxProcessPerFrame = 50;
            int processed = 0;

            while (PendingFolders.Count > 0 && processed < maxProcessPerFrame)
            {
                string currentPath = PendingFolders[0];
                PendingFolders.RemoveAt(0);

                if (!NodeMap.ContainsKey(currentPath))
                {
                    FolderNode parentNode = GetParentNode(currentPath);
                    FolderNode currentNode = new FolderNode(Path.GetFileName(currentPath));

                    NodeMap.Add(currentPath, currentNode);
                    if (parentNode != null)
                    {
                        parentNode.Children.Add(currentNode);

                        currentNode.SetDepth(parentNode.Depth + 1);
                    }
                    else nodeRoot.Children.Add(currentNode);

                    // 收集文件信息
                    ProcessFiles(currentPath, currentNode);

                    // 获取子文件夹
                    string[] subFolders = AssetDatabase.GetSubFolders(currentPath);
                    PendingFolders.AddRange(subFolders);
                }

                processed++;
            }

            if (PendingFolders.Count == 0)
            {
                // 扫描完成后统一计算高度 【后续遍历】
                CalculateHeights(nodeRoot);
            }
        }
        
        private static void ProcessFiles(string folderPath, FolderNode node)
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".png" || ext == ".jpg")
                {
                    node.GetImagePaths().Add(file);
                    string tempPath = CopyToTempAsset(file);
                    if (!string.IsNullOrEmpty(tempPath))
                    {
                        node.AddTextureReference(tempPath);
                        node.AddImageTexture(new ImageTexture()
                        {
                            Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(tempPath),
                            Name = tempPath
                        });
                    }
                }
                else if (ext == ".txt")
                {
                    node.GetTextInfo().strings.AddRange(File.ReadAllLines(file));
                }
            }
        }

        private static string CopyToTempAsset(string sourcePath)
        {
            // 生成唯一文件名（包含原始路径哈希）
            string fileName = $"{Path.GetFileNameWithoutExtension(sourcePath)}_{sourcePath.GetHashCode():X8}";

            // 使用AssetDatabase.CopyAsset保持导入设置
            string tempSourcePath = AssetDatabase.GenerateUniqueAssetPath($"{TempRoot}/source_{fileName}{Path.GetExtension(sourcePath)}");
            File.Copy(sourcePath, tempSourcePath);
            AssetDatabase.ImportAsset(tempSourcePath, ImportAssetOptions.ForceUpdate);

            // 配置TextureImporter
            TextureImporter importer = AssetImporter.GetAtPath(tempSourcePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.maxTextureSize = 2048;
                importer.SaveAndReimport();
            }
            
            return tempSourcePath;
        }

        private static void CalculateHeights(FolderNode node)
        {
            if (node == null) return;

            int maxChildHeight = 0;
            foreach (var child in node.Children)
            {
                CalculateHeights(child);
                maxChildHeight = Math.Max(maxChildHeight, child.Height);
            }

            node.SetHeight(maxChildHeight + 1);
        }
        
        private static FolderNode GetParentNode(string path)
        {
            string parentPath = Path.GetDirectoryName(path)?.Replace("\\", "/");
            return NodeMap.GetValueOrDefault(parentPath);
        }
        
        // 清理临时Assets
        public static void CleanTempAssets()
        {
            string tempDir = "Assets/TempAssets~";
            if (Directory.Exists(tempDir))
            {
                AssetDatabase.DeleteAsset(tempDir);
                AssetDatabase.Refresh();
            }
        }

        public static FolderNode GetNodeByPath(string path) => PathToNodeMap.TryGetValue(path, out var node) ? node : null;
    }
}
#endif