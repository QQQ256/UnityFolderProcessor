using UnityEngine;
using FolderProcessor;

namespace Example.Scripts.Launch
{
    public class Launch : MonoBehaviour
    {
        void Start()
        {
            // InitializeParameters parameters = new InitializeParameters
            // {
            //     ClearImagePathOnLoad = true
            // };
            // FolderProcess.Initialize(parameters);
            // FolderProcess.Instance.DefaultLoadAllData(FolderProcess.DefaultRootNodeName, () =>
            // {
            //     FolderNode rootNode = FolderProcess.GetFolderNodeWithFolderOriginalName("FolderA");
            //     if (rootNode != null)
            //     {
            //         FindObjectOfType<Canvas>().transform.GetChild(0).GetComponent<Panel>().Setup(rootNode);
            //     }
            // });
            //
            // string testPath = @"D:\SharedTest\XiaoPeng-16-AIGC";
            // FolderProcess.Instance.CustomLoadAllData("Test", testPath, () =>
            // {
            //     FolderNode testNode = FolderProcess.GetFolderNodeWithFolderOriginalName("SignFolder", "Test");
            //     if (testNode != null)
            //     {
            //         Debug.Log(testNode.TextureCount);
            //     }
            //
            //     FolderNodeRoot rootNode = FolderProcess.GetFolderNodeRoot("Test");
            //     if (rootNode != null)
            //     {
            //         Debug.Log(rootNode.RootNode.ChildCount);
            //         if (FolderProcess.RemoveFolderNode("Test", "SignFolder"))
            //         {
            //             Debug.Log(rootNode.RootNode.ChildCount);
            //         }
            //     }
            // });

            FolderProcessDriver driver = gameObject.AddComponent<FolderProcessDriver>();
            driver.Initialize(() => { Debug.Log("loaded"); });

            driver.CreateFolderProcess("Test",
                new InitializeParameters()
                {
                    LoadPath = @"D:\SharedTest\XiaoPeng-16-AIGC"
                }, () =>
                {
                    Debug.Log("Test FolderProcess loaded");
                    var node = driver.GetFolderNodeByFolderProcessName("Test", "SignFolder");
                    Debug.Log(node.TextureCount);
                });
        }
    }
}