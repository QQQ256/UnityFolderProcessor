using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace FolderProcessor
{
    internal static class FolderNodeUtility
    {
        private static readonly string PatternStart = @"^\d+[。\.]|。$";
        
        /// <summary>
        /// 过滤掉文件夹名前缀的数字和点号
        /// 例如1.FolderName 或 2。FolderName 会被处理为 FolderName
        /// 后续直接通过 GetFolderNodeWithFolderOriginalName("FolderName") 访问
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string TrimPrefix(string input)
        {
            return Regex.Replace(input, PatternStart, "");
        }
        
        internal static void SortFolderByPrefixNumber(DirectoryInfo[] directoryInfos)
        {
            Array.Sort(directoryInfos, delegate (DirectoryInfo x, DirectoryInfo y)
            {
                // 抓取第一串数字进行比较
                var regex = new Regex(@"\d+");

                // 尝试从文件夹名中抓取数字
                var xMatch = regex.Match(x.Name);
                var yMatch = regex.Match(y.Name);

                // 如果两个目录名都含数字，则比较数字
                if (xMatch.Success && yMatch.Success)
                {
                    var xNumber = int.Parse(xMatch.Value);
                    var yNumber = int.Parse(yMatch.Value);
                    return xNumber.CompareTo(yNumber);
                }

                // 如果一个含数字，另一个不含，则数字的目录排在前
                if (xMatch.Success) return -1;
                if (yMatch.Success) return 1;

                // 如果两个都不含数字，则按字典顺序比较
                return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
            });
        }
        
        internal static IEnumerator LoadImageAsync(string filePath, Action<Texture2D> onComplete)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("File not found: " + filePath);
                yield break;
            }

            using UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + filePath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                onComplete?.Invoke(texture);
            }
            else
            {
                Debug.LogError($"Failed to load image {filePath}: {www.error}");
                onComplete?.Invoke(null);
            }
        }
        
        internal static List<FolderNode> RecursivelyCollectChildFolderNodes(FolderNode node)
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
                    result.AddRange(RecursivelyCollectChildFolderNodes(child));
                }
            }

            return result;
        }
    }
}