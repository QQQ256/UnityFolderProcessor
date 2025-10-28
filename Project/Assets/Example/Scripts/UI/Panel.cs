using System.Collections.Generic;
using UnityEngine;
using FolderProcessor;

namespace Example.Scripts.UI
{
    public class Panel : MonoBehaviour
    {
        public RectTransform content;
        public GameObject prefab;

        public void Setup(FolderNode rootNode)
        {
            if (rootNode == null)
            {
                Debug.LogError("rootNode is null");
                return;
            }
            
            Debug.Log(rootNode.ToString());
            
            GameObject temp = Instantiate(prefab, content);
            var tempPrefab = temp.GetComponent<Prefab>();
            tempPrefab.Setup(rootNode);

            BFS(rootNode);
            
            prefab.gameObject.SetActive(false);
        }

        private void BFS(FolderNode rootNode)
        {
            // BFS 队列初始化
            Queue<FolderNode> nodeQueue = new Queue<FolderNode>();
            nodeQueue.Enqueue(rootNode);

            float prefabScale = .9f;

            while (nodeQueue.Count > 0)
            {
                FolderNode currentNode = nodeQueue.Dequeue();

                // 遍历子节点
                if (currentNode.Children != null && currentNode.ChildCount > 0)
                {
                    foreach (FolderNode child in currentNode.Children)
                    {
                        Debug.Log(child.ToString());

                        // 实例化子节点
                        GameObject childObj = Instantiate(prefab, content);
                        childObj.GetComponent<RectTransform>().localScale = Vector3.one * prefabScale;
                        var childPrefab = childObj.GetComponent<Prefab>();
                        childPrefab.Setup(child);

                        // 将子节点加入队列，继续 BFS
                        nodeQueue.Enqueue(child);
                    }

                    prefabScale -= .1f;
                }
            }
        }
    }
}
