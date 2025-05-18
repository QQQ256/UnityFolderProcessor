using Example.Scripts.UI;
using UnityEngine;
using WXQToolKits.FolderKit.Runtime;

namespace Example.Scripts.Launch
{
    public class Launch : MonoBehaviour
    {
        void Start()
        {
            FolderProcess.Instance.LoadAllData(() =>
            {
                FolderNode rootNode = FolderProcess.Instance.GetFolderNodeWithFolderOriginalName("FolderA");
                if (rootNode != null)
                {
                    FindObjectOfType<Canvas>().transform.GetChild(0).GetComponent<Panel>().Setup(rootNode);
                }
            });
        }
    }
}
