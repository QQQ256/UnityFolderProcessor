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
        private bool[] _nodeFoldouts; // ����ÿ���ڵ���۵�״̬

        public override void OnInspectorGUI()
        {
            var asset = (FolderNodeAsset)target;

            // ������Ϣ��ʾ
            EditorGUILayout.LabelField("�ڵ�����", asset.TreeElements.Count.ToString());

            // ��ʼ���۵�״̬����
            if (_nodeFoldouts == null || _nodeFoldouts.Length != asset.TreeElements.Count)
            {
                _nodeFoldouts = new bool[asset.TreeElements.Count];
            }

            // ������ͼ
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // �������нڵ�
            for (int i = 0; i < asset.TreeElements.Count; i++)
            {
                var node = asset.TreeElements[i];

                // �ڵ������
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                _nodeFoldouts[i] = EditorGUILayout.Foldout(_nodeFoldouts[i],
                    $"{node.Name} (Depth:{node.Depth})", true);

                // ��ʾ��������
                EditorGUILayout.LabelField($"Textures: {node.GetTextureCount()}",
                    GUILayout.Width(100));

                EditorGUILayout.EndHorizontal();

                // չ������ʾ������������
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

            // ��ȡ������������·��
            foreach (var texRef in node.GetTextureReferenceList())
            {
                string textureReferencePath = texRef.Replace('\\', '/'); // �滻��б��Ϊ��б��
                Debug.Log(textureReferencePath);
                DrawTextureReferenceItem(textureReferencePath);
            }

            EditorGUI.indentLevel--;
        }

        private void DrawTextureReferenceItem(string assetPath)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            // 1. ��ʾ��������ͼ
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

            // 2. ��ʾ·���Ͳ�����ť
            EditorGUILayout.BeginVertical();

            // ·����ʾ���ɵ����
            if (GUILayout.Button(assetPath, EditorStyles.label))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(assetPath));
            }

            // ������ť
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