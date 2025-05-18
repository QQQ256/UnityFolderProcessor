using System.Collections.Generic;
using UnityEngine;
using WXQToolKits.FolderKit.Runtime;

namespace WXQToolKits.FolderKit.Editor
{
    [CreateAssetMenu (fileName = "FolderNodeAsset", menuName = "FolderNode Asset", order = 1)]
    public class FolderNodeAsset : ScriptableObject
    {
        [SerializeField] private List<FolderNode> treeElements = new List<FolderNode>();
        
        // 根节点引用
        [SerializeField] private FolderNode rootNode;
        
        internal List<FolderNode> TreeElements
        {
            get => treeElements;
            set => treeElements = value;
        }
        
        // 存储整个树结构
        public void StoreTree(FolderNode root)
        {
            rootNode = root;
            // treeElements = rootNode.Children;
            treeElements.Clear();
            FlattenTree(root, treeElements);
        }
        
        // 将树结构扁平化为列表
        private void FlattenTree(FolderNode node, List<FolderNode> list)
        {
            if (node == null) return;
            
            list.Add(node);
            foreach (var child in node.Children)
            {
                FlattenTree(child, list);
            }
        }
        
        // 从存储重建树结构
        public FolderNode RebuildTree()
        {
            if (rootNode == null && treeElements.Count > 0)
            {
                // 重建父子关系
                var nodeMap = new Dictionary<string, FolderNode>();
                foreach (var node in treeElements)
                {
                    nodeMap[node.Name] = node;
                    node.Children.Clear();
                }
                
                foreach (var node in treeElements)
                {
                    if (node.Parent != null && nodeMap.TryGetValue(node.Parent.Name, out var parent))
                    {
                        parent.Children.Add(node);
                    }
                }
                
                // 找到根节点
                rootNode = treeElements.Find(n => n.Parent == null);
            }
            
            return rootNode;
        }
    }
}
