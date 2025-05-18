using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace WXQToolKits.FolderKit.Runtime
{
    public  partial class FolderProcess
    {
        private IEnumerator LoadAllImages(FolderNode rootNode, Action action)
        {
            Queue<FolderNode> nodeQueue = new Queue<FolderNode>();
            nodeQueue.Enqueue(rootNode);

            // BFS
            while (nodeQueue.Count > 0)
            {
                FolderNode currentNode = nodeQueue.Dequeue();

                if (currentNode.GetImagePaths() != null)
                {
                    //Debug.Log("start play with node" + currentNode.Name);
                    foreach (string imagePath in currentNode.GetImagePaths())
                    {
                        Texture2D texture = null;
                        yield return StartCoroutine(LoadImageAsync(imagePath, (loadedTexture) => texture = loadedTexture));

                        if (texture != null)
                        {
                            string textureName = Path.GetFileNameWithoutExtension(imagePath);
                            
                            currentNode.GetImageInfoList().Add(new ImageTexture { Name = textureName, Texture = texture });
                            texture = null;
                            GC.Collect();
                        }
                    }
                    currentNode.GetImagePaths().Clear();
                }

                foreach (FolderNode child in currentNode.Children)
                {
                    //Debug.Log(currentNode.Name + "�ĺ���" + child.Name + "�������");
                    //Debug.Log(child.imagePaths.Count);
                    nodeQueue.Enqueue(child);
                }
            }

            action();
        }


        private IEnumerator LoadImageAsync(string filePath, Action<Texture2D> onComplete)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("File not found: " + filePath);
                yield break;
            }

            using UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + filePath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                onComplete?.Invoke(texture);
            }
            else
            {
                Debug.LogError($"Failed to load image {filePath}: {www.error}");
                onComplete?.Invoke(null);
            }
        }
    }
}
