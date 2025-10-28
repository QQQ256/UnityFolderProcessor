using System.Collections.Generic;
using UnityEngine;

namespace FolderProcessor
{
    public static class FolderNodeExtensions
    {
        public static FolderNode GetChildAt(this FolderNode parent, int index)
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
        
        /// <summary>
        /// 获取列表中指定范围的子节点（安全版）
        /// </summary>
        /// <param name="source">源列表</param>
        /// <param name="startIndex">起始索引（包含）</param>
        /// <param name="count">要获取的数量</param>
        /// <returns>返回范围内的子节点列表，若参数无效则返回空列表</returns>
        public static List<FolderNode> GetChildNodeInRangeSafely(
            this List<FolderNode> source, 
            int startIndex, 
            int count)
        {
            if (source == null || source.Count == 0)
            {
                Debug.LogError("Source list is null or empty");
                return new List<FolderNode>();
            }

            if (startIndex < 0 || startIndex >= source.Count)
            {
                Debug.LogError($"Start index {startIndex} is out of range [0, {source.Count - 1}]");
                return new List<FolderNode>();
            }

            // 计算实际可获取的数量
            int validCount = Mathf.Min(count, source.Count - startIndex);
            return source.GetRange(startIndex, validCount);
        }
    }
}
