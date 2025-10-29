using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolderProcessor
{
    public class FolderProcessDriver : MonoSingleton<FolderProcessDriver>
    {
        private static Dictionary<string, FolderProcess> _folderProcesses;
        
        private const string DefaultFolderProcessName = "DefaultFolderProcess";
        
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
            foreach (FolderProcess folderProcess in _folderProcesses.Values)
            {
                folderProcess.ClearAllFolderNodeData();
            }
            _folderProcesses.Clear();
            _folderProcesses = null;
        }

        public void CreateFolderProcess(string folderProcessName, InitializeParameters initializeParameters = null, Action loadedAction = null)
        {
            if (initializeParameters == null)
                initializeParameters = DefaultInitializeParameters;

            CreateFolderProcessInternal(folderProcessName, loadedAction, initializeParameters);
        }

        public FolderNode GetFolderNodeByFolderProcessName(string folderProcessName, string folderName)
        {
            if (_folderProcesses.TryGetValue(folderProcessName, out FolderProcess folderProcess))
            {
                return folderProcess.GetFolderNodeWithFolderOriginalName(folderName);
            }

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