using System.Collections.Generic;
using Example.Scripts.Case2;
using Example.Scripts.UI;
using FolderProcessor;
using UnityEngine;
using UnityEngine.UI;

namespace Example.Scripts
{
    public class UseCase2 : MonoBehaviour
    {
        public RectTransform content;
        public GameObject horizontalScrollViewPrefab;
        public GameObject prefab;
        public Case2UI caseUI;
        
        public void Setup(FolderNode folderNode)
        {
            horizontalScrollViewPrefab.gameObject.SetActive(false);
            prefab.gameObject.SetActive(false);
            
            Bfs(folderNode);
        }

        private void Bfs(FolderNode rootNode)
        {
            // BFS 队列初始化
            Queue<FolderNode> nodeQueue = new Queue<FolderNode>();
            nodeQueue.Enqueue(rootNode);

            int paddingLeft = 0;
            int paddingToAdd = 100;
            while (nodeQueue.Count > 0)
            {
                FolderNode currentNode = nodeQueue.Dequeue();
                int height = currentNode.Height;

                // 遍历子节点
                if (currentNode.ChildCount > 0)
                {
                    GameObject parent = Instantiate(horizontalScrollViewPrefab, content);
                    RectTransform childContentRect = parent.transform.Find("Viewport/Content").GetComponent<RectTransform>();
                    HorizontalLayoutGroup layoutGroup = childContentRect.GetComponent<HorizontalLayoutGroup>();
                    
                    layoutGroup.padding.left = paddingLeft;                    
                    parent.SetActive(true);

                    foreach (FolderNode child in currentNode.Children)
                    {
                        Debug.Log(child.ToString());

                        // 实例化子节点
                        GameObject childObj = Instantiate(prefab, childContentRect);
                        childObj.SetActive(true);
                        var childPrefab = childObj.GetComponent<Case2ClickablePrefab>();
                        childPrefab.Setup(child, OnFolderNodeClickedHandler);

                        // 将子节点加入队列，继续 BFS
                        nodeQueue.Enqueue(child);
                    }

                    paddingLeft += paddingToAdd;
                }
            }
        }

        private void OnFolderNodeClickedHandler(FolderNode folderNode)
        {
            caseUI.Setup(folderNode);
        }
    }
}
