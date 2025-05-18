using System.Collections.Generic;
using UnityEngine;
using WXQToolKits.FolderKit.Singleton;

namespace WXQToolKits.FolderKit.Runtime
{
    public partial class FolderProcess : MonoSingleton<FolderProcess>
    {
        public static FolderNode RootNode => _folderNode1;
        
        private static FolderNode _folderNode1;

        private static readonly Dictionary<string, FolderNode> FilesDictionary = new Dictionary<string, FolderNode>();

        public FolderNode GetFolderNodeWithFolderOriginalName(string folderName)
        {
            if (FilesDictionary.TryGetValue(folderName, out FolderNode targetNode))
            {
                return targetNode;
            }

            Debug.Log($"fail to get folderNode: {folderName}");
            return null;
        }
    }
}
