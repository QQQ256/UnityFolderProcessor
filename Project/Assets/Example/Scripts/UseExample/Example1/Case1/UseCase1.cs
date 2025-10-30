using FolderProcessor;
using UnityEngine;
using UnityEngine.UI;

namespace Example.Scripts
{
    public class UseCase1 : MonoBehaviour
    {
        public RawImage rawImage;
        public Text rawImageName;
        
        public void Setup(FolderNode folderNode)
        {
            if (folderNode.IsContainsImageTextureList)
            {
                ImageTexture imageTexture = folderNode.GetImageTextureWithIndex(0);
                rawImage.texture = imageTexture.Texture;
                rawImageName.text = imageTexture.TextureName;
            }
        }
    }
}
