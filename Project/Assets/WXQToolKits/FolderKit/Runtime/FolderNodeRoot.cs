using System.Collections.Generic;

namespace FolderProcessor
{
    public class FolderNodeRoot
    {
        public FolderNode RootNode { get; }
        public string RootNodeName => RootNode.Name;
        public List<FolderNode> RootNodeChild => RootNode.Children;
        public int RootNodeChildCount => RootNode.Children.Count;

        /// <summary>
        /// key - 父文件夹名
        /// value - 文件夹内容节点，包含图片List，文本文件，视频List等
        /// </summary>
        public Dictionary<string, FolderNode> FilesDictionary { get; } = new();

        public FolderNodeRoot(string folderNodeName)
        {
            RootNode = new FolderNode(folderNodeName);
        }
        
        public void AddFolderNodeToDictionary(string folderName, FolderNode folderNode)
        {
            FilesDictionary.TryAdd(folderName, folderNode);
        }
    }
}