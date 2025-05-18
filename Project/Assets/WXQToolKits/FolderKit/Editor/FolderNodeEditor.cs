#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using WXQToolKits.FolderKit.Runtime;

namespace WXQToolKits.FolderKit.Editor
{
    [CustomEditor(typeof(FolderNodeAsset))]
    public class FolderNodeEditor : UnityEditor.Editor
    {
        private Vector2 _scrollPos;
        private bool[] _nodeFoldouts; // 控制每个节点的折叠状态

        public override void OnInspectorGUI()
        {
            var asset = (FolderNodeAsset)target;

            // 基础信息显示
            EditorGUILayout.LabelField("节点总数", asset.TreeElements.Count.ToString());

            // 初始化折叠状态数组
            if (_nodeFoldouts == null || _nodeFoldouts.Length != asset.TreeElements.Count)
            {
                _nodeFoldouts = new bool[asset.TreeElements.Count];
            }

            // 滚动视图
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // 遍历所有节点
            for (int i = 0; i < asset.TreeElements.Count; i++)
            {
                var node = asset.TreeElements[i];

                // 节点标题栏
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                _nodeFoldouts[i] = EditorGUILayout.Foldout(_nodeFoldouts[i],
                    $"{node.Name} (Depth:{node.Depth})", true);

                // 显示纹理数量
                EditorGUILayout.LabelField($"Textures: {node.GetTextureCount()}",
                    GUILayout.Width(100));

                EditorGUILayout.EndHorizontal();

                // 展开后显示纹理引用详情
                if (_nodeFoldouts[i] && node.GetTextureCount() > 0)
                {
                    DrawTextureReferences(node);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawTextureReferences(FolderNode node)
        {
            EditorGUI.indentLevel++;

            // 获取所有纹理引用路径
            foreach (var texRef in node.GetTextureReferenceList())
            {
                string textureReferencePath = texRef.Replace('\\', '/'); // 替换反斜杠为正斜杠
                Debug.Log(textureReferencePath);
                DrawTextureReferenceItem(textureReferencePath);
            }

            EditorGUI.indentLevel--;
        }

        private void DrawTextureReferenceItem(string assetPath)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            // 1. 显示纹理缩略图
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (tex != null)
            {
                Rect thumbRect = GUILayoutUtility.GetRect(60, 60);
                EditorGUI.DrawPreviewTexture(thumbRect, tex);
            }
            else
            {
                EditorGUILayout.LabelField("Missing", GUILayout.Width(60));
            }

            // 2. 显示路径和操作按钮
            EditorGUILayout.BeginVertical();

            // 路径显示（可点击）
            if (GUILayout.Button(assetPath, EditorStyles.label))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(assetPath));
            }

            // 操作按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select", EditorStyles.miniButtonLeft))
            {
                Selection.activeObject = tex;
            }

            if (GUILayout.Button("Locate", EditorStyles.miniButtonMid))
            {
                EditorUtility.RevealInFinder(assetPath);
            }

            if (GUILayout.Button("Copy Path", EditorStyles.miniButtonRight))
            {
                GUIUtility.systemCopyBuffer = assetPath;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

    // class FolderNodeTreeView : 
}
#endif