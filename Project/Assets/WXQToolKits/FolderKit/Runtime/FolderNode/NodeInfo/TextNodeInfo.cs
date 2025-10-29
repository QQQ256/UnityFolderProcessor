using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolderProcessor
{
    [Serializable]
    public class TextNodeInfo : INodeInfo
    {
        public bool IsEmpty => strings is { Count: 0 };
        public void Clear() => strings.Clear();
        
        [SerializeField]
        private List<string> strings = new();

        public void SetTextString(List<string> str) => strings = str;

        public string GetTextInfoString()
        {
            if (IsEmpty)
                return string.Empty;
            return string.Join("\n", strings.ToString());
        }
    }
}