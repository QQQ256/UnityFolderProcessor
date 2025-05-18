using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace WXQToolKits.FolderKit.Editor
{
    public static class TextureLoader
    {
        #region 核心工具方法
        public static Texture2D LoadTexture2D(string tempPath)
        {
            // 备用方案 - 直接读取字节
            try
            {
                byte[] fileData = File.ReadAllBytes(tempPath);
                Texture2D newTex = new Texture2D(100, 100);
                if (newTex.LoadImage(fileData)) // 自动解码PNG/JPG
                {
                    Debug.LogWarning($"通过字节加载Texture2D: {tempPath}");
                    return newTex;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"备用加载方案失败: {e.Message}");
            }

            return null;
        }
        
        // 支持的图片格式检查
        public static bool IsSupportedImageFile(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg";
        }
        #endregion
    }
}
