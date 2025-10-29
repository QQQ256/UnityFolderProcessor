using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace FolderProcessor
{
    [Serializable]
    public class ImageTexture
    {
        /// <summary>
        /// Texture2D 在序列化时会因为层级太深有Warning
        /// </summary>
        public Texture2D Texture;
        public string TextureName;
    }
    
    public class ImageNodeInfo : INodeInfo
    {
        public bool IsEmpty => _imageTextureList.Count == 0;
        
        public void Clear()
        {
            foreach (var imageTexture in _imageTextureList)
            {
                UnityEngine.Object.Destroy(imageTexture.Texture);
            }
            
            _imageTextureList.Clear();
        }
        
        [SerializeField]
        private List<ImageTexture> _imageTextureList = new List<ImageTexture>();

        public List<Texture2D> Texture2DList =>
            _imageTextureList.Where(t => t.Texture != null).Select(t => t.Texture).ToList();
        
        public int TextureCount => _imageTextureList.Count;

        
        
        
        
        public Texture2D GetTextureWithIndex(int index)
        {
            if (index < 0 || index >= TextureCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _imageTextureList[index].Texture;
        }

        public ImageTexture GetImageTextureWithIndex(int index)
        {
            if (index < 0 || index >= TextureCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _imageTextureList[index];
        }

        public void AddImageTexture(ImageTexture imageTexture)
        {
            if (imageTexture != null)
            {
                _imageTextureList.Add(imageTexture);
            }
            else
            {
                Debug.LogError("ImageTexture is null!");
            }
        }
    }
}