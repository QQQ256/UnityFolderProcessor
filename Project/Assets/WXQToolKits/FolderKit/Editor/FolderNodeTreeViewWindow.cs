#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using WXQToolKits.FolderKit.Runtime;

namespace WXQToolKits.FolderKit.Editor
{
    public class FolderNodeTreeViewWindow : EditorWindow
    {
        [SerializeField] private FolderNodeAsset storageAsset;
        
        [MenuItem("Tools/Folder Scanner New")]
        public static void ShowWindow()
        {
            GetWindow<FolderNodeTreeViewWindow>("Folder Scanner");
        }

        private FolderNode _editorRoot;
        private TreeViewState _treeViewState;
        private FolderNodeTreeView _folderNodeTreeView;
        private SearchField _searchField;
        private bool _initialized;

        private bool _isScanning;
        private float _progress;
        private readonly List<string> _pendingFolders = new List<string>();
        private readonly Dictionary<string, FolderNode> _nodeMap = new Dictionary<string, FolderNode>();

        void OnGUI()
        {
            InitIfNeed();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Scan Folder Now!", GUILayout.Width(150)))
            {
                string path = Path.Combine(Application.dataPath, "StreamingAssets/Resources/Data");
                if (!string.IsNullOrEmpty(path))
                {
                    StartScanning(path);
                }
            }

            if (_isScanning)
            {
                Rect rect = EditorGUILayout.GetControlRect();
                EditorGUI.ProgressBar(rect, _progress, "Scanning...");
                GUILayout.Label($"Processed: {_pendingFolders.Count} folders");
            }

            EditorGUILayout.EndHorizontal();

            if (_folderNodeTreeView != null)
            {
                Rect treeRect =
                    GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                _folderNodeTreeView.OnGUI(treeRect);
            }
            
            if (GUILayout.Button("Save Data", GUILayout.Width(100)))
            {
                SaveData();
            }
            
            if (GUILayout.Button("Load Data", GUILayout.Width(100)))
            {
                LoadData();
            }

            if (GUILayout.Button("Expand Treeview", GUILayout.Width(150)))
            {
                if (_folderNodeTreeView != null)
                    _folderNodeTreeView.ExpandAll();
            }
        }

        #region Init Method

        private void InitIfNeed()
        {
            if (!_initialized)
            {
                if (_treeViewState == null)
                {
                    _treeViewState = new TreeViewState();
                }

                _searchField = new SearchField();

                _initialized = true;
            }
        }

        #endregion
        
        
        private void SaveData()
        {
            if (_editorRoot == null) return;
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Folder Data",
                "FolderData",
                "asset",
                "Select location to save folder data");
            
            if (!string.IsNullOrEmpty(path))
            {
                var asset = CreateInstance<FolderNodeAsset>();
                asset.StoreTree(_editorRoot);
                
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                
                storageAsset = asset;
                EditorUtility.DisplayDialog("Success", "Folder data saved successfully!", "OK");
            }
        }


        private void LoadData()
        {
            string path = EditorUtility.OpenFilePanel(
                "Load Folder Data",
                Application.dataPath,
                "asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                var asset = AssetDatabase.LoadAssetAtPath<FolderNodeAsset>(relativePath);
                
                if (asset != null)
                {
                    storageAsset = asset;
                    _editorRoot = asset.RebuildTree();
                    _folderNodeTreeView = new FolderNodeTreeView(_treeViewState, _editorRoot);
                    _folderNodeTreeView.Reload();
                    _folderNodeTreeView.ExpandAll();
                }
            }
        }

        #region Scan and Read Data

        private void StartScanning(string rootPath)
        {
            _editorRoot = new FolderNode(Path.GetFileName(rootPath));
            _pendingFolders.Clear();
            _nodeMap.Clear();

            // string relativePath = "Assets" + rootPath.Substring(Application.dataPath.Length);
            // _pendingFolders.Add(relativePath);
            TempAssetGenerator.ProcessScanning(rootPath, _editorRoot);

            _folderNodeTreeView = new FolderNodeTreeView(_treeViewState, _editorRoot);

            // _isScanning = true;
            // EditorApplication.update -= AsyncScan;
            // EditorApplication.update += AsyncScan;
        }

        private void AsyncScan()
        {
            const int maxProcessPerFrame = 50;
            int processed = 0;

            while (_pendingFolders.Count > 0 && processed < maxProcessPerFrame)
            {
                string currentPath = _pendingFolders[0];
                _pendingFolders.RemoveAt(0);

                if (!_nodeMap.ContainsKey(currentPath))
                {
                    FolderNode parentNode = GetParentNode(currentPath);
                    FolderNode currentNode = new FolderNode(Path.GetFileName(currentPath));

                    _nodeMap.Add(currentPath, currentNode);
                    if (parentNode != null)
                    {
                        parentNode.Children.Add(currentNode);

                        currentNode.SetDepth(parentNode.Depth + 1);
                    }
                    else _editorRoot.Children.Add(currentNode);

                    // 收集文件信息
                    ProcessFiles(currentPath, currentNode);

                    // 获取子文件夹
                    string[] subFolders = AssetDatabase.GetSubFolders(currentPath);
                    _pendingFolders.AddRange(subFolders);
                }

                processed++;
                _progress = 1f - (float)_pendingFolders.Count / (_pendingFolders.Count + processed);
                Repaint();
            }

            if (_pendingFolders.Count == 0)
            {
                // 扫描完成后统一计算高度 【后续遍历】
                CalculateHeights(_editorRoot);

                // 读取图片
                StartImageLoading(_editorRoot);

                _isScanning = false;
                EditorApplication.update -= AsyncScan;
                _folderNodeTreeView.Reload();
                _folderNodeTreeView.ExpandAll();
            }
        }

        private void StartImageLoading(FolderNode editorRoot)
        {
            // 开始批量Asset操作
            AssetDatabase.StartAssetEditing();
            
            try
            {
                Queue<FolderNode> nodeQueue = new Queue<FolderNode>();
                nodeQueue.Enqueue(editorRoot);

                while (nodeQueue.Count > 0)
                {
                    FolderNode currentNode = nodeQueue.Dequeue();
                    if (currentNode.GetImagePaths() == null) continue;

                    foreach (string imagePath in currentNode.GetImagePaths())
                    {
                        // 生成临时Asset
                        Texture2D texture = TextureLoader.LoadTexture2D(imagePath);

                        if (texture != null)
                        {
                            currentNode.AddImageTexture(new ImageTexture
                            {
                                Name = Path.GetFileNameWithoutExtension(imagePath),
                                Texture = texture
                            });
                            Debug.Log($"成功加载: {texture.name} 纹理");
                        }
                    }
                    
                    // asset.AddFolderTextures(currentNode, Application.dataPath);

                    foreach (FolderNode child in currentNode.Children)
                    {
                        nodeQueue.Enqueue(child);
                    }
                }
            }
            finally
            {
                // 结束批量操作
                AssetDatabase.StopAssetEditing();
                // // 延迟清理（可选）
                // EditorApplication.delayCall += TempAssetGenerator.CleanTempAssets;
            }
        }

        // 后序遍历计算高度
        private void CalculateHeights(FolderNode node)
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

        private FolderNode GetParentNode(string path)
        {
            string parentPath = Path.GetDirectoryName(path)?.Replace("\\", "/");
            return _nodeMap.GetValueOrDefault(parentPath);
        }

        private void ProcessFiles(string folderPath, FolderNode node)
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".png" || ext == ".jpg")
                {
                    node.GetImagePaths().Add(file);
                }
                else if (ext == ".txt")
                {
                    node.GetTextInfo().strings.AddRange(File.ReadAllLines(file));
                }
            }
        }

        #endregion
    }
}
#endif