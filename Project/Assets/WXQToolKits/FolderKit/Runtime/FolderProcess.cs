using System;
using System.Collections.Generic;
using FolderProcessor.Singleton;
using UnityEngine;

namespace FolderProcessor
{
    public partial class FolderProcess : MonoSingleton<FolderProcess>
    {
        private static InitializeParameters _initializeParameters;
        private static bool _initialized;
        
        /// <summary>
        /// 用户自定义读取的根目录文件夹节点
        /// </summary>
        private static readonly Dictionary<string, FolderNodeRoot> RootFolderNodes = new();

        private static FolderNodeRoot DefaultFolderNode => RootFolderNodes[DefaultRootNodeName];
        
        public const string DefaultRootNodeName = "DefaultRootNode";

        public static void Initialize(InitializeParameters parameters)
        {
            CheckInitializeParameters(parameters);
            _initialized = true;
        }
        
        public static FolderNodeRoot GetFolderNodeRoot(string rootNodeName = DefaultRootNodeName)
        {
            if (RootFolderNodes.TryGetValue(rootNodeName, out var node))
            {
                return node;
            }

            throw new Exception($"Failed to get FolderNodeRoot: {rootNodeName}");
        }

        public static FolderNode GetFolderNodeWithFolderOriginalName(string folderName, string rootNodeName = DefaultRootNodeName)
        {
            if (!RootFolderNodes.TryGetValue(rootNodeName, out var node))
            {
                throw new Exception($"Failed to get FolderNodeRoot: {rootNodeName}");
            }
            
            var filesDictionary = node.FilesDictionary;
            if (filesDictionary.TryGetValue(folderName, out FolderNode targetNode))
            {
                return targetNode;
            }

            throw new Exception($"Failed to get FolderNode: {folderName}");
        }

        private static void CheckInitializeParameters(InitializeParameters parameters)
        {
            if (_initialized)
            {
                throw new Exception($"{nameof(FolderProcess)} is initialized yet.");
            }
            
            if (parameters == null)
                throw new Exception($"{nameof(FolderProcess)} create parameters is null.");

            _initializeParameters = parameters;
        }

        private static void ClearAllFolderNodeData()
        {
            foreach (var rootFolderNode in RootFolderNodes)
            {
                foreach (var folderNode in rootFolderNode.Value.RootNodeChild)
                {
                    folderNode.Clear();
                }
            }
            
            Debug.Log("All FolderNode data cleared.");
        }

        private void OnDestroy()
        {
            ClearAllFolderNodeData();
        }
    }
}