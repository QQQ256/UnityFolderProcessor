using System;
using Example.Scripts.UI;
using UnityEngine;

namespace Example.Scripts
{
    public class UseExample1 : MonoBehaviour
    {
        public Panel examplePanel;

        private void Start()
        {
            examplePanel.CallUseCases();
        }
    }
}
