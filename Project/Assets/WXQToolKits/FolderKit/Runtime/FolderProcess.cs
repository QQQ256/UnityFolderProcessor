using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolderProcessor
{
    [Serializable]
    public partial class FolderProcess
    {
        private InitializeParameters _initializeParameters;
        private string _folderProcessName;
        private bool _initialized;
        
        /// <summary>
        /// 用户自定义读取的根目录文件夹节点
        /// </summary>
        private Dictionary<string, FolderNode> _nodeDictionary;
        
        private static MonoBehaviour _driverInstance;

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
    }
}