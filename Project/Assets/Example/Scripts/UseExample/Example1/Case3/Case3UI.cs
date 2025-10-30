using FolderProcessor;
using UnityEngine;
using UnityEngine.UI;

namespace Example.Scripts
{
    public class Case3UI : MonoBehaviour
    {
        public RawImage rawImage;
        public Text rawImageName;

        public Text title;
        public Text content;
        
        public void Setup(FolderNode folderNode)
        {
            title.text = "Node Name: " + folderNode.Name;
            if (folderNode.IsContainsImageTextureList)
            {
                ImageTexture imageTexture = folderNode.GetImageTextureWithIndex(0);
                rawImage.texture = imageTexture.Texture;
                rawImageName.text = imageTexture.TextureName;
            }

            content.text = folderNode.GetTextInfoString();
        }
    }
}
