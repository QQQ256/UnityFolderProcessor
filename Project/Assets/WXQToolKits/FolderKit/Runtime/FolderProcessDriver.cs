using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolderProcessor
{
    public class FolderProcessDriver : MonoSingleton<FolderProcessDriver>
    {
        public FolderNode RootFolderNode
        {
            get
            {
                // 优先获取自定义FolderProcess中的RootFolderNode
                if (string.IsNullOrEmpty(_customFolderProcessName))
                {
                    return GetFolderNodeByFolderProcessName(DefaultFolderProcessName);
                }
                
                // 其次是用户自定的FolderProcess
                return GetFolderNodeByFolderProcessName(_customFolderProcessName);
            }
        }
        private const string DefaultFolderProcessName = "DefaultFolderProcess";
        private static string _customFolderProcessName = string.Empty;
        
        private static Dictionary<string, FolderProcess> _folderProcesses;
        private static readonly InitializeParameters DefaultInitializeParameters = new InitializeParameters
        {
            ClearImagePathOnLoad = true,
            LoadPath = Application.streamingAssetsPath + "/Resources"
        };
        private static bool _isInitialized;

        public void Initialize(Action loadedAction = null)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("FolderProcessDriver is already initialized.");
                return;
            }
            
            _isInitialized = true;
            _folderProcesses = new Dictionary<string, FolderProcess>();
            
            CreateDefaultFolderProcess(loadedAction);
        }

        public void OnDestroy()
        {
            if (_folderProcesses != null)
            {
                foreach (FolderProcess folderProcess in _folderProcesses.Values)
                {
                    folderProcess.ClearAllFolderNodeData();
                }
                _folderProcesses.Clear();
                _folderProcesses = null;
            }
        }

        public void CreateFolderProcess(string folderProcessName, InitializeParameters initializeParameters = null, Action loadedAction = null)
        {
            if (initializeParameters == null)
                initializeParameters = DefaultInitializeParameters;

            if (folderProcessName != DefaultFolderProcessName)
                _customFolderProcessName = folderProcessName;

            CreateFolderProcessInternal(folderProcessName, loadedAction, initializeParameters);
        }

        /// <summary>
        /// 默认获取默认FolderProcess中的FolderNode
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="folderProcessName"></param>
        /// <returns></returns>
        public FolderNode GetFolderNodeByFolderProcessName(string folderName, string folderProcessName = DefaultFolderProcessName)
        {
            if (_folderProcesses.TryGetValue(folderProcessName, out FolderProcess folderProcess))
            {
                return folderProcess.GetFolderNodeWithFolderOriginalName(folderName);
            }

            Debug.LogWarning("FolderProcess not found: " + folderProcessName);
            return null;
        }
        
        
        
        
        
        
        
        private void CreateFolderProcessInternal(string folderProcessName, Action loadedAction = null, InitializeParameters initializeParameters = null)
        {
            var folderProcess = new FolderProcess();
            folderProcess.Initialize(initializeParameters, this, folderProcessName);
            folderProcess.LoadAsync(loadedAction);

            if (_folderProcesses.TryAdd(folderProcessName, folderProcess))
            {
                Debug.Log("Create FolderProcess: " + folderProcessName);
            }
            else
            {
                Debug.LogError("FolderProcess already exists: " + folderProcessName);
            }
        }

        private void CreateDefaultFolderProcess(Action loadedAction) => CreateFolderProcess(DefaultFolderProcessName, DefaultInitializeParameters, loadedAction);
    }
}