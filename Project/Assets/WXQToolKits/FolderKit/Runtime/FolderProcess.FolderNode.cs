using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WXQToolKits.FolderKit.Runtime
{
    [Serializable]
    public class FolderNode
    {
        private FolderNode _parent;
        [SerializeField] private string name;
        [SerializeField] private int height;
        [SerializeField] private int depth;
        [SerializeField] private ImageInfo imageInfo = new ImageInfo();
        [SerializeField] private VideoInfo videoInfo = new VideoInfo();
        [SerializeField] private TextInfo textInfo = new TextInfo();

        [SerializeField] private List<FolderNode> children = new List<FolderNode>();

        public string Name => name;
        public FolderNode Parent => _parent;
        public int Height => height;
        public int Depth => depth;
        public List<FolderNode> Children => children;
        public int ChildCount => children.Count;

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<color=green> FolderNode: {Name}");
            sb.AppendLine($"©À©¤ Depth: {Depth}");
            sb.AppendLine($"©À©¤ Height: {Height}");
            sb.AppendLine($"©À©¤ Children: {ChildCount}");

            sb.AppendLine("©À©¤ Resources:");
            sb.AppendLine($"©¦  ©À©¤ Text: {GetTextCount()} entries");
            sb.AppendLine($"©¦  ©À©¤ Images: {GetTextureCount()} textures");
            sb.AppendLine($"©¦  ©¸©¤ Videos: {GetVideoInfoList().Count} videos");

            if (Parent != null)
            {
                sb.AppendLine($"©¸©¤ Parent: {Parent.Name}");
            }
            else
            {
                sb.AppendLine("©¸©¤ Parent: [Root]");
            }

            sb.AppendLine("</color>");

            return sb.ToString();
        }


        public bool IsFolderContainsVideo => GetVideoInfoList().Count > 0;
        public bool IsFolderContainsText => textInfo.strings.Count > 0;
        public bool IsFolderContainsImageTextureList => GetImageInfoList().Count > 0;


        public TextInfo GetTextInfo() => textInfo;
        public int GetTextCount() => textInfo.strings.Count;
        public string GetTextInfoString() => string.Join("\n", textInfo.strings);

        public List<string> GetImagePaths() => ImagePaths;
        public ImageInfo GetImageInfo() => imageInfo;

        public List<ImageTexture> GetImageInfoList() => imageInfo.imageTextureList;
        public List<Texture2D> GetExtractedTextures() => imageInfo.GetExtractedTextures;
        public ImageTexture GetImageTexture(int index) => imageInfo.GetImageTexture(index);
        public Texture2D GetTexture(int index) => imageInfo.GetTexture(index);
        public int GetTextureCount() => imageInfo.TextureCount;
        public void AddImageTexture(ImageTexture imageTexture) => imageInfo.AddImageTexture(imageTexture);

#if UNITY_EDITOR
        public List<string> GetTextureReferenceList() => imageInfo.ReferenceList;
        public void AddTextureReference(string refPath) => imageInfo.AddTextureReference(refPath);
#endif

        public VideoInfo GetVideoInfo() => videoInfo;
        public List<string> GetVideoInfoList() => videoInfo.videoFullPathList;
        public void AddVideoFullPath(string videoFullPath) => videoInfo.videoFullPathList.Add(videoFullPath);
        public string GetVideoFullPathWithVideoName(string videoName) => videoInfo.GetVideoNamePath(videoName);
        public string GetVideoFullPath(int index) => videoInfo.Get(index);


        private List<string> ImagePaths { get; } = new List<string>();

        public FolderNode(string name, FolderNode parent = null)
        {
            this.name = name;
            this._parent = parent;
        }

        public void SetHeight(int h) => this.height = h;
        public void SetDepth(int d) => this.depth = d;
    }

    [Serializable]
    public class TextInfo
    {
        public List<string> strings = new List<string>();
    }

    [Serializable]
    public class ImageInfo
    {
#if UNITY_EDITOR
        public readonly List<string> ReferenceList = new List<string>();
#endif
        public List<ImageTexture> imageTextureList = new List<ImageTexture>();

        public List<Texture2D> GetExtractedTextures =>
            imageTextureList.Where(t => t.Texture != null).Select(t => t.Texture).ToList();

        public int TextureCount => imageTextureList.Count;

        public Texture2D GetTexture(int index)
        {
            if (index < 0 || index >= TextureCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return imageTextureList[index].Texture;
        }

        public ImageTexture GetImageTexture(int index)
        {
            if (index < 0 || index >= TextureCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return imageTextureList[index];
        }
        
#if UNITY_EDITOR
        public void AddTextureReference(string refPath)
        {
            ReferenceList.Add(refPath);
        }
#endif

        public void AddImageTexture(ImageTexture imageTexture)
        {
            if (imageTexture != null)
            {
                imageTextureList.Add(imageTexture);
            }
            else
            {
                Debug.LogError("ImageTexture is null!");
            }
        }
    }

    [Serializable]
    public class VideoInfo
    {
        public List<string> videoFullPathList = new List<string>();
        private readonly Dictionary<string, string> _videoNamePathDic = new Dictionary<string, string>();

        private int VideoCount => videoFullPathList.Count;

        public string GetVideoNamePath(string videoName)
        {
            return _videoNamePathDic.GetValueOrDefault(videoName);
        }

        public string Get(int index)
        {
            if (index < 0 || index >= VideoCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return videoFullPathList[index];
        }
    }

    [Serializable]
    public class ImageTexture
    {
        public Texture2D Texture;
        public string Name;
    }
}