using UnityEngine;
using FolderProcessor;
using UnityEngine.SceneManagement;

namespace Example.Scripts.Launch
{
    public class Boot : MonoBehaviour
    {
        void Start()
        {
            GameObject g = new GameObject("FolderProcessDriver");
            FolderProcessDriver driver = g.AddComponent<FolderProcessDriver>();
            driver.Initialize(() =>
            {
                Debug.Log("FolderProcessDriver initialized");
                SceneManager.LoadScene("SampleScene");
            });

            // driver.CreateFolderProcess("Test",
            //     new InitializeParameters()
            //     {
            //         LoadPath = @"D:\SharedTest\XiaoPeng-16-AIGC"
            //     }, () =>
            //     {
            //         Debug.Log("Test FolderProcess loaded");
            //         var node = driver.GetFolderNodeByFolderProcessName("SignFolder", "Test");
            //         Debug.Log(node.TextureCount);
            //     });
        }
    }
}