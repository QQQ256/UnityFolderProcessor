using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FolderProcessor
{
    [Serializable]
    public class VideoNodeInfo : INodeInfo
    { 
        public bool IsEmpty => videoFullPathList.Count == 0;

        public void Clear() => videoFullPathList.Clear();
        
        public List<string> videoFullPathList = new();
        public int VideoCount => videoFullPathList.Count;

        public string GetVideoNamePath(string videoName)
        {
            return videoFullPathList.Find(videoFullPath =>
                videoName == Path.GetFileNameWithoutExtension(videoFullPath));
        }

        public string GetVideoFullPathWithIndex(int index)
        {
            if (index < 0 || index >= VideoCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return videoFullPathList[index];
        }
        
        public void AddVideoFullPath(string videoFullPath)
        {
            if (!string.IsNullOrEmpty(videoFullPath))
            {
                videoFullPathList.Add(videoFullPath);
            }
            else
            {
                Debug.LogError("Video full path is null or empty! " + videoFullPath);
            }
        }
    }
}