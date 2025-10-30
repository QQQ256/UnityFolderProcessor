using FolderProcessor;
using UnityEngine;
using UnityEngine.UI;

namespace Example.Scripts.Case2
{
    public class Case2UI : MonoBehaviour
    {
        public Text txtHeight;
        public Text txtDepth;
        public Text txtTextureCount;
        public Text txtFolderNodeToString;
        
        public void Setup(FolderNode folderNode)
        {
            txtDepth.text = "Depth: " + folderNode.Depth;
            txtHeight.text = "Height: " + folderNode.Height;
            txtTextureCount.text = "TextureCount: " + folderNode.TextureCount;
            txtFolderNodeToString.text = folderNode.ToString();
        }
    }
}
