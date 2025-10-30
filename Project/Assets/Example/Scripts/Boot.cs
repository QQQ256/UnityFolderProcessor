using UnityEngine;
using FolderProcessor;
using UnityEngine.SceneManagement;

namespace Example.Scripts
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
        }
    }
}