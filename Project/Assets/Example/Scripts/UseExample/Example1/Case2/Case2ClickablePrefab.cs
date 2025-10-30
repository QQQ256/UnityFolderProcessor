using System;
using FolderProcessor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Example.Scripts
{
    public class Case2ClickablePrefab : MonoBehaviour, IPointerClickHandler
    {
        public Text textComponent;
        
        private FolderNode _currentFolderNode;
        private Action<FolderNode> _onNodeClicked;
        private string NodeName => _currentFolderNode.Name;

        public void Setup(FolderNode folderNode, Action<FolderNode> onNodeClicked)
        {
            _currentFolderNode = folderNode;
            _onNodeClicked = onNodeClicked;
            
            textComponent.text = NodeName;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_currentFolderNode != null)
            {
                _onNodeClicked?.Invoke(_currentFolderNode);
            }
        }
    }
}
