using System;
using System.Collections.Generic;
using FolderProcessor.Singleton;
using UnityEngine;

namespace FolderProcessor
{
    public class FolderProcessDriver : MonoSingleton<FolderProcessDriver>
    {
        private static readonly Dictionary<string, FolderProcess> FolderProcesses = new();
        
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
            CreateDefaultFolderProcess(loadedAction);
        }

        public void OnDestroy()
        {
            foreach (FolderProcess folderProcess in FolderProcesses.Values)
            {
                folderProcess.ClearAllFolderNodeData();
            }
        }

        public void CreateFolderProcess(string folderProcessName, InitializeParameters initializeParameters = null, Action loadedAction = null)
        {
            if (initializeParameters == null)
                initializeParameters = DefaultInitializeParameters;

            CreateFolderProcessInternal(folderProcessName, loadedAction, initializeParameters);
        }

        public FolderNode GetFolderNodeByFolderProcessName(string folderProcessName, string folderName)
        {
            if (FolderProcesses.TryGetValue(folderProcessName, out FolderProcess folderProcess))
            {
                return folderProcess.GetFolderNodeWithFolderOriginalName(folderName);
            }

            return null;
        }
        
        
        
        
        
        
        
        private void CreateFolderProcessInternal(string folderProcessName, Action loadedAction = null, InitializeParameters initializeParameters = null)
        {
            var defaultFolderProcess = new FolderProcess();
            defaultFolderProcess.Initialize(initializeParameters, this, folderProcessName);
            defaultFolderProcess.LoadAsync(loadedAction);

            if (FolderProcesses.TryAdd(folderProcessName, defaultFolderProcess))
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