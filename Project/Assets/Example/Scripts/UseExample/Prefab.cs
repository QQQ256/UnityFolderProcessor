using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FolderProcessor;

namespace Example.Scripts.UI
{
    public class Prefab : MonoBehaviour
    {
        public RawImage rawImage;
        public Text textForFolderNodeData;
        public Text textForFolderNodeText;
        public Text textForChildFolderNodeText;
        
        public void Setup(FolderNode folderNode)
        {
            if (folderNode == null)
            {
                Debug.LogError("FolderNode is null");
                return;
            }
            
            textForFolderNodeData.text = folderNode.ToString();

            if (folderNode.IsFolderContainsText)
            {
                textForFolderNodeText.text = "Text from node: " + folderNode.GetTextInfoString();
            }
            else
            {
                textForFolderNodeText.text = "No text for folder node";
            }

            if (folderNode.IsContainsImageTextureList)
            {
                rawImage.texture = folderNode.GetTextureWithIndex(0);
            }

            if (folderNode.ChildCount > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Child Folder Nodes: \n");
                foreach (FolderNode childNode in folderNode.Children)
                {
                    sb.Append(childNode.Name).Append('\n');
                }

                if (sb.Length > 0)
                {
                    sb.Length--;
                }
                
                textForChildFolderNodeText.text = sb.ToString();
            }
            else
            {
                textForChildFolderNodeText.text = "No child folder node";
            }
        }
    }
}
