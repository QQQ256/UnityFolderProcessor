using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using WXQToolKits.FolderKit.Runtime;

namespace WXQToolKits.FolderKit.Editor
{
    public class FolderNodeTreeView : TreeView
    {
        private readonly FolderNode _root;
        private readonly Dictionary<int, FolderNode> _nodeMap = new Dictionary<int, FolderNode>();

        private readonly GUIStyle _richTextStyle = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(5, 5, 2, 2)
        };

        public FolderNodeTreeView(TreeViewState state, FolderNode root) : base(state)
        {
            this._root = root;
            showAlternatingRowBackgrounds = true;
            Reload();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var node = FindNode(args.item.id);
            if (node == null)
            {
                Debug.LogError("FolderNode is null!");
                return;
            }

            // 原始文本渲染区域 (宽度减少给按钮留空间)
            Rect contentRect = args.rowRect;
            contentRect.x += GetContentIndent(args.item);
            contentRect.width -= 100; // 为按钮预留空间

            // 构建摘要文本
            string summary = $"<b>{node.Name}</b>";
            int textureCount = node.GetTextureCount();
            int textCount = node.GetTextCount();
            if (node.GetTextInfo().strings.Count > 0)
            {
                summary += $" <color=#777>| 文本行:{textCount}| 纹理数:{textureCount}</color>";
            }

            // 渲染文本
            GUI.Label(contentRect, summary, _richTextStyle);

            // 添加"附加到面板"按钮
            Rect buttonRect = new Rect(
                args.rowRect.x + args.rowRect.width - 90,
                args.rowRect.y,
                70,
                args.rowRect.height);

            if (node.IsFolderContainsText || node.IsFolderContainsImageTextureList)
            {
                if (GUI.Button(buttonRect, "附加数据"))
                {
                    Debug.Log("附加数据按钮被点击，名称是：" + node.Name);
                    // AttachToDataPanel(node); // 调用附加方法
                    foreach (var li in node.GetTextureReferenceList())
                    {
                        Debug.Log(li);
                    }
                }
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            var myRootItem = new TreeViewItem(-1, -1, "Root Node");
            _nodeMap.TryAdd(myRootItem.id, _root);

            BuildTreeItems(_root, myRootItem, 0);

            return myRootItem;
        }

        protected override void DoubleClickedItem(int id)
        {
            var item = FindItem(id, rootItem);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.AssetPathToGUID(item.displayName))
            ));
        }


        private void BuildTreeItems(FolderNode node, TreeViewItem parent, int currentDepth)
        {
            var item = new TreeViewItem(node.GetHashCode(), currentDepth, node.Name);

            _nodeMap[item.id] = node; // 当前TreeViewItem映射到对应的FolderNode
            // Debug.Log(item.id + " " + node.Name);
            parent.AddChild(item);

            foreach (var child in node.Children)
            {
                BuildTreeItems(child, item, currentDepth + 1);
            }
        }

        private FolderNode FindNode(int itemID)
        {
            return _nodeMap.GetValueOrDefault(itemID);
        }


        // // Get all textures and current index
        // int textureCount = node.GetTextureCount();
        // Texture2D[] textures = new Texture2D[textureCount];
        // for (int i = 0; i < textureCount; i++)
        // {
        //     textures[i] = node.GetTexture(i);
        // }
        //
        // // Calculate the available height for our content (accounting for padding)
        // float availableHeight = rowRectHeight + standardIconSize; // Small padding
        //
        // // Display area for texture and controls
        // Rect textureRect = new Rect(contentRect.x, contentRect.y + (rowRectHeight - standardIconSize) / 2, 0, 0);
        // string currentTextureName = "";
        //
        // if (textureCount > 0)
        // {
        //     Texture2D currentTexture = textures[0];
        //
        //     // Calculate maximum size that fits within the row
        //     float maxHeight = availableHeight;
        //     float maxWidth = 100f; // Reasonable max width for texture in a treeview
        //
        //     // Calculate scale factor to fit within row height while maintaining aspect ratio
        //     float heightScale = maxHeight / currentTexture.height;
        //     float widthScale = maxWidth / currentTexture.width;
        //     float scale = Mathf.Min(heightScale, widthScale);
        //
        //     // Apply minimum scale of 1.0 if texture is smaller than our max dimensions
        //     scale = Mathf.Max(scale, 1.0f);
        //
        //     textureRect.width = Mathf.Min(currentTexture.width * scale, maxWidth);
        //     textureRect.height = Mathf.Min(currentTexture.height * scale, maxHeight);
        //
        //     // Center vertically
        //     textureRect.y = contentRect.y + (rowRectHeight - textureRect.height) / 2;
        //
        //     // Draw the texture
        //     GUI.DrawTexture(textureRect, currentTexture);
        // }
        //
        // // Adjust text position based on texture display
        // float textX = textureCount > 0 ? textureRect.xMax + 4f : contentRect.x;
        // float textWidth = contentRect.width - (textX - contentRect.x);
    }
}