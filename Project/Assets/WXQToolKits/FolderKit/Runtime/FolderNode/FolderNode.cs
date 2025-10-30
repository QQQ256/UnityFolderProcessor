using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FolderProcessor
{
    
    [Serializable]
    public class FolderNode
    {
        // 修改属性为可序列化字段
        [SerializeField] private FolderNode parent;
        [SerializeField] private string name;
        [SerializeField] private int height;
        [SerializeField] private int depth;
        [SerializeField] private ImageNodeInfo imageNodeInfo = new();
        [SerializeField] private VideoNodeInfo videoNodeInfo = new();
        [SerializeField] private TextNodeInfo textNodeInfo = new();

        [SerializeField] private List<FolderNode> children = new();

        // 修改属性访问器
        public string Name => name;
        public FolderNode Parent => parent;
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
            this.parent = parent;
        }

        public void SetHeight(int h) => this.height = h;
        public void SetDepth(int d) => this.depth = d;

        public void Clear()
        {
            imageNodeInfo.Clear();
            videoNodeInfo.Clear();
            textNodeInfo.Clear();
            
            Debug.Log("FolderNode Data cleared: " + Name);
        }

        public override string ToString()
        {
            // 基础信息
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<color=green> FolderNodeName: {Name}");
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
        public bool IsFolderContainsVideo => !videoNodeInfo.IsEmpty;
        public bool IsFolderContainsText => !textNodeInfo.IsEmpty;
        public bool IsContainsImageTextureList => !imageNodeInfo.IsEmpty;


        // public TextInfo GetTextInfo() => textInfo;
        // public int GetTextCount() => textInfo.strings.Count;
        // public List<string> GetTextList() => textInfo.strings;

        // 文字方法
        public void InitTextInfo(List<string> strings) => textNodeInfo.SetTextString(strings);
        public string GetTextInfoString() => textNodeInfo.GetTextInfoString();
        
        
        // 图片读取相关方法
        // 图片路径
        public List<string> ImagePaths { get; private set; } = new List<string>();
        public void ClearImagePaths() => ImagePaths.Clear();
        
        // 图片信息，将磁盘中的图片读取并存储至内存中
        public ImageTexture GetImageTextureWithIndex(int index) => imageNodeInfo.GetImageTextureWithIndex(index);
        public Texture2D GetTextureWithIndex(int index) => imageNodeInfo.GetTextureWithIndex(index);
        public List<Texture2D> GetTextureList() => imageNodeInfo.Texture2DList;
        public int TextureCount => imageNodeInfo.TextureCount;
        public void AddImageTexture(ImageTexture imageTexture) => imageNodeInfo.AddImageTexture(imageTexture);

        // 视频读取相关方法
        public string GetVideoFullPathWithVideoName(string videoName) => videoNodeInfo.GetVideoNamePath(videoName);
        public string GetVideoFullPathWithIndex(int index) => videoNodeInfo.GetVideoFullPathWithIndex(index);
        public int VideoCount => videoNodeInfo.VideoCount;
        public void AddVideoFullPath(string videoFullPath) => videoNodeInfo.AddVideoFullPath(videoFullPath);
    }
}