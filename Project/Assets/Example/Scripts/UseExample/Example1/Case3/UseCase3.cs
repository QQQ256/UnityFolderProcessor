using System;
using System.Collections.Generic;
using System.IO;
using FolderProcessor;
using UnityEngine;
using UnityEngine.UI;

namespace Example.Scripts
{
    public class UseCase3 : MonoBehaviour
    {
        public RectTransform content;
        public Button loadButton;
        public Transform verticalRect;
        public GameObject horizontalScrollViewPrefab;
        public GameObject prefab;
        public Case3UI case3UI;

        private void Start()
        {
            horizontalScrollViewPrefab.gameObject.SetActive(false);
            prefab.gameObject.SetActive(false);
            
            loadButton.onClick.AddListener(LoadNewFolderProcess);
        }

        private void LoadNewFolderProcess()
        {
            verticalRect.gameObject.SetActive(false);
            
            string folderProcessName = "Case3FolderProcess";
            string folderNodeRootName = "Case3Folder";
            string folderProcessPath = Path.Combine(Application.dataPath, folderNodeRootName);
            FolderProcessDriver.Instance.CreateFolderProcess(folderProcessName, 
                // 加载新的文件夹处理流程，指定要读取的文件夹路径
                new InitializeParameters()
            {
                LoadPath = folderProcessPath
            }, // 初始化完成后的回调，参考Case2的逻辑
                () =>
            {
                // 通过自己设定的FolderProcess名称获取对应的RootFolderNode
                FolderNode rootFolderNode = FolderProcessDriver.Instance.
                    GetFolderNodeByFolderProcessName(folderProcessName, folderProcessName);

                if (rootFolderNode != null)
                {
                    Setup(rootFolderNode);
                    
                    verticalRect.gameObject.SetActive(true);
                    loadButton.gameObject.SetActive(false);
                }
            });
        }

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
                        // Debug.Log(child.ToString());

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
            case3UI.Setup(folderNode);
        }
    }
}
