using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace WXQToolKits.FolderKit.Runtime
{
    /// <summary>
    /// Partial class for FolderNode Creation
    /// </summary>
    public partial class FolderProcess
    {
        static readonly string[] SupportedImageExtensions = new string[] { ".png", ".jpg", ".jpeg", ".JPG", ".PNG" };
        static readonly string[] SupportedTextExtensions = new string[] { ".txt" };
        static readonly string[] VideoExtensions = new string[] { ".mp4", ".mov" };

        private List<string> _filteredFolderNameList = new List<string>();


        #region Load Dta from Folder
        
        /// <summary>
        /// Load All Data From StreamingAssets/Resources Folder
        /// </summary>
        /// <param name="action"></param>
        public void LoadAllData(Action action = null)
        {
            _folderNode1 = new FolderNode("Root");
            FilesDictionary["Root"] = _folderNode1;
            StartCoroutine(AddChildFoldersCoroutine(_folderNode1, Application.streamingAssetsPath + "/Resources", 0, action));
        }

        public void LoadAllData(string absPath, Action action = null)
        {
            _folderNode1 = new FolderNode("Root");
            FilesDictionary["Root"] = _folderNode1;
            StartCoroutine(AddChildFoldersCoroutine(_folderNode1, absPath, 0, action));
        }

        public void LoadDataFromFolders(string folderName, Action action)
        {
            var newNode = CreateNewFolderNode(folderName, _folderNode1);
            StartCoroutine(AddChildFoldersCoroutine(newNode, Application.streamingAssetsPath + "/" + folderName, 0, action));
        }
        #endregion

        #region Add Child Folders via Coroutine Function

        private IEnumerator AddChildFoldersCoroutine(FolderNode parentNode, string parentFolderPath, int currentDepth, Action action = null)
        {
            var directoryInfo = new DirectoryInfo(parentFolderPath);
            var directories = directoryInfo.GetDirectories();

            parentNode.SetDepth(currentDepth);

            SortByName(directories);

            // get rid of searching filtered folders
            int directoriesCount = directories.Count(dir => !_filteredFolderNameList.Contains(TrimPrefix(dir.Name.Substring(dir.Name.LastIndexOf("\\", StringComparison.Ordinal) + 1))));

            int processedCount = 0;

            foreach (var directory in directories)
            {
                string key = TrimPrefix(directory.FullName.Substring(directory.FullName.LastIndexOf("\\", StringComparison.Ordinal) + 1));

                if (_filteredFolderNameList != null && _filteredFolderNameList.Contains(key))
                {
                    Debug.Log($"Skipping folder due to filter: {key}");
                    continue;
                }

                FolderNode folderNode = new FolderNode(key, parentNode);

                if (!FilesDictionary.TryGetValue(key, out _))
                {
                    FilesDictionary[key] = folderNode;
                }

                yield return StartCoroutine(ProcessFilesInDirectory(directory.FullName, folderNode));

                parentNode.Children.Add(folderNode);

                yield return StartCoroutine(AddChildFoldersCoroutine(folderNode, directory.FullName, currentDepth + 1));

                processedCount++;

                parentNode.SetHeight(Math.Max(parentNode.Height, folderNode.Height + 1));

                if (action != null && processedCount == directoriesCount)
                {
                    Debug.Log("All FolderNode created.");
                    StartCoroutine(LoadAllImages(parentNode, action));
                }
            }
        }


        private IEnumerator ProcessFilesInDirectory(string directoryPath, FolderNode folderNode)
        {
            string[] files = Directory.GetFiles(directoryPath);

            foreach (var filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (SupportedTextExtensions.Contains(extension))
                {
                    string[] lines = File.ReadAllLines(filePath);

                    folderNode.GetTextInfo().strings = new List<string>(lines);
                }
                else if (SupportedImageExtensions.Contains(extension))
                {
                    folderNode.GetImagePaths().Add(filePath);
                }
                else if (VideoExtensions.Contains(extension))
                {
                    folderNode.AddVideoFullPath(filePath);
                }
            }
            yield return null;
        }
        #endregion

        
        static readonly string PatternStart = @"^\d+[、\.]|。$";
        private static string TrimPrefix(string input)
        {
            string trimmed = Regex.Replace(input, PatternStart, "");
            return trimmed;
        }
        
        
        
        
        
        #region Set Filter Folders
        public void SetFilterFolders(List<string> filteredFolderNameList)
        {
            /*
             * example:
             * FolderProcess.Instance.SetFilterFolders(new List<string>()
                    {
                        "FolderName 1",
                        "FolderName 2"
                    });
             */
            _filteredFolderNameList = filteredFolderNameList;
        }
        #endregion
    }
}
