using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace FolderProcessor
{
    public interface IInfo
    {
        bool IsEmpty { get; }
        void Clear();
    }
    
    [Serializable]
    public class FolderNode
    {
        // 修改属性为可序列化字段
        private FolderNode _parent;
        [SerializeField] private string name;
        [SerializeField] private int height;
        [SerializeField] private int depth;
        [SerializeField] private ImageInfo imageInfo = new ImageInfo();
        [SerializeField] private VideoInfo videoInfo = new VideoInfo();
        [SerializeField] private TextInfo textInfo = new TextInfo();

        [SerializeField] private List<FolderNode> children = new List<FolderNode>();

        // 修改属性访问器
        public string Name => name;
        public FolderNode Parent => _parent;
        public int Height => height;
        public int Depth => depth;
        public List<FolderNode> Children => children;
        public List<FolderNode> ParentChildrenNodes
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Children;
                }
                
                throw new Exception("Parent's children list is null");
            }
        }
        public int ChildCount => children.Count;
        
        public FolderNode(string name, FolderNode parent = null)
        {
            this.name = name;
            this._parent = parent;
        }

        public void SetHeight(int h) => this.height = h;
        public void SetDepth(int d) => this.depth = d;

        public void Clear()
        {
            imageInfo.Clear();
            videoInfo.Clear();
            textInfo.Clear();
            
            Debug.Log("FolderNode Data cleared: " + Name);
        }

        public override string ToString()
        {
            // 基础信息
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<color=green> FolderNode: {Name}");
            sb.AppendLine($"├─ Depth: {Depth}");
            sb.AppendLine($"├─ Height: {Height}");
            sb.AppendLine($"├─ Children: {ChildCount}");

            // 资源统计
            sb.AppendLine($"├─ Resources:");
            // sb.AppendLine($"│  ├─ Text: {textInfo.strings} entries");
            sb.AppendLine($"│  ├─ Images: {TextureCount} textures");
            sb.AppendLine($"│  └─ Videos: {VideoCount} videos");

            // 父节点信息（如果存在）
            if (Parent != null)
            {
                sb.AppendLine($"└─ Parent: {Parent.Name}");
            }
            else
            {
                sb.AppendLine("└─ Parent: [Root]");
            }

            sb.AppendLine("</color>");

            return sb.ToString();
        }
        
        // 资源标记
        public bool IsFolderContainsVideo => videoInfo.IsEmpty;
        public bool IsFolderContainsText => textInfo.IsEmpty;
        public bool IsFolderContainsImageTextureList => imageInfo.IsEmpty;


        // public TextInfo GetTextInfo() => textInfo;
        // public int GetTextCount() => textInfo.strings.Count;
        // public List<string> GetTextList() => textInfo.strings;

        // 文字方法
        public void InitTextInfo(List<string> strings) => textInfo.SetTextString(strings);
        public string GetTextInfoString() => textInfo.GetTextInfoString();
        
        
        // 图片读取相关方法
        // 图片路径
        public List<string> ImagePaths { get; private set; } = new List<string>();
        // TODO: 这里可以再弄个配置之类的，是否需要去清空路径列表
        public void ClearImagePaths() => ImagePaths.Clear();
        
        // 图片信息，将磁盘中的图片读取并存储至内存中
        public ImageTexture GetImageTextureWithIndex(int index) => imageInfo.GetImageTextureWithIndex(index);
        public Texture2D GetTextureWithIndex(int index) => imageInfo.GetTextureWithIndex(index);
        public List<Texture2D> GetTextureList() => imageInfo.Texture2DList;
        public int TextureCount => imageInfo.TextureCount;
        public void AddImageTexture(ImageTexture imageTexture) => imageInfo.AddImageTexture(imageTexture);

        // 视频读取相关方法
        public string GetVideoFullPathWithVideoName(string videoName) => videoInfo.GetVideoNamePath(videoName);
        public string GetVideoFullPathWithIndex(int index) => videoInfo.GetVideoFullPathWithIndex(index);
        public int VideoCount => videoInfo.VideoCount;
        public void AddVideoFullPath(string videoFullPath) => videoInfo.AddVideoFullPath(videoFullPath);
    }

    [Serializable]
    public class TextInfo : IInfo
    {
        public bool IsEmpty => _strings is { Count: 0 };
        public void Clear() => _strings.Clear();
        
        private List<string> _strings = new();

        public void SetTextString(List<string> strings) => _strings = strings;

        public string GetTextInfoString()
        {
            if (IsEmpty)
                return string.Empty;
            return string.Join("\n", _strings.ToString());
        }
    }

    [Serializable]
    public class ImageInfo : IInfo
    {
        public bool IsEmpty => ImageTextureList.Count == 0;
        
        public void Clear()
        {
            foreach (var imageTexture in ImageTextureList)
            {
                UnityEngine.Object.Destroy(imageTexture.Texture);
            }
            
            ImageTextureList.Clear();
        }
        
        public List<ImageTexture> ImageTextureList { get; private set; } = new List<ImageTexture>();

        /// <summary>
        /// 将 ImageTextureList 转换为 Texture2D 列表
        /// </summary>
        public List<Texture2D> Texture2DList =>
            ImageTextureList.Where(t => t.Texture != null).Select(t => t.Texture).ToList();
        
        public int TextureCount => ImageTextureList.Count;

        public Texture2D GetTextureWithIndex(int index)
        {
            if (index < 0 || index >= TextureCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return ImageTextureList[index].Texture;
        }

        public ImageTexture GetImageTextureWithIndex(int index)
        {
            if (index < 0 || index >= TextureCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return ImageTextureList[index];
        }

        public void AddImageTexture(ImageTexture imageTexture)
        {
            if (imageTexture != null)
            {
                ImageTextureList.Add(imageTexture);
            }
            else
            {
                Debug.LogError("ImageTexture is null!");
            }
        }
    }

    [Serializable]
    public class VideoInfo : IInfo
    { 
        public bool IsEmpty => videoFullPathList.Count == 0;

        public void Clear() => videoFullPathList.Clear();
        
        public List<string> videoFullPathList = new List<string>();
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

    [Serializable]
    public class ImageTexture
    {
        public Texture2D Texture;
        public string TextureName;
    }
}