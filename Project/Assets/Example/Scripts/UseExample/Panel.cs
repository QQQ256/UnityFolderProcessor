using System.Collections.Generic;
using UnityEngine;
using FolderProcessor;

namespace Example.Scripts.UI
{
    public class Panel : MonoBehaviour
    {
        public UseCase1 useCase1;
        public UseCase2 useCase2;
        public UseCase3 useCase3;

        public void CallUseCases()
        {
            // 使用方式1：直接通过特定文件夹名称进行查询
            // 默认查询的是 DefaultFolderProcessName 这个 FolderProcessor 字典中的文件夹
            UseCase1();
            
            // 使用方式2：从根节点开始遍历整个文件夹树，展示所有文件夹节点
            UseCase2();
            
            // 使用方式3：指定读取另外一个文件夹处理流程
            UseCase3();
        }

        private void UseCase1()
        {
            FolderNode folderNodeNameC = FolderProcessDriver.Instance.GetFolderNodeByFolderProcessName("FolderC");
            if (folderNodeNameC != null)
            {
                useCase1.Setup(folderNodeNameC);
            }
        }

        private void UseCase2()
        {
            FolderNode rootFolderNode = FolderProcessDriver.Instance.RootFolderNode;
            if (rootFolderNode != null)
            {
                useCase2.Setup(rootFolderNode);
            }
        }

        private void UseCase3()
        {
        }
    }
}
