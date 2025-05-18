using System.Collections.Generic;
using UnityEngine;

namespace WXQToolKits.FolderKit.Extension
{
    public static class FolderNodeExtensions
    {
        public static FolderKit.Runtime.FolderNode GetChildAt(this FolderKit.Runtime.FolderNode parent, int index)
        {
            if (parent == null)
            {
                Debug.LogError("Parent FolderNode is null");
                return null;
            }

            if (parent.Children == null || parent.Children.Count == 0)
            {
                Debug.LogError("Parent has no children");
                return null;
            }

            if (index < 0 || index >= parent.Children.Count)
            {
                Debug.LogError($"Index {index} is out of range [0, {parent.Children.Count - 1}]");
                return null;
            }

            return parent.Children[index];
        }
        
        public static bool IsFolderHasChildren(this FolderKit.Runtime.FolderNode folderNode)
        {
            if (folderNode == null)
            {
                Debug.LogError("FolderNode is null");
                return false;
            }

            return folderNode.Children is { Count: > 0 };
        }

        public static string GetFolderNodeParentName(this FolderKit.Runtime.FolderNode folderNode)
        {
            if (folderNode == null)
            {
                Debug.LogError("FolderNode is null");
                return string.Empty;
            }

            if (folderNode.Parent == null)
            {
                Debug.LogError("Parent FolderNode is null");
                return string.Empty;
            }

            return folderNode.Parent.Name;
        }
        
        public static List<FolderKit.Runtime.FolderNode> GetRangeSafely(
            this List<FolderKit.Runtime.FolderNode> source, 
            int startIndex, 
            int count)
        {
            if (source == null || source.Count == 0)
            {
                Debug.LogError("Source list is null or empty");
                return new List<FolderKit.Runtime.FolderNode>();
            }

            if (startIndex < 0 || startIndex >= source.Count)
            {
                Debug.LogError($"Start index {startIndex} is out of range [0, {source.Count - 1}]");
                return new List<FolderKit.Runtime.FolderNode>();
            }

            int validCount = Mathf.Min(count, source.Count - startIndex);
            return source.GetRange(startIndex, validCount);
        }
    }
}
