using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class TouchControlLayer : CanvasWindowLayer
    {
        [SerializeField] private Button _reloadButton;
        [SerializeField] private Button _attackButton;


        public Button ReloadButton => _reloadButton;
        public Button AttackButton => _attackButton;


        public void SetLayerStatus(bool status)
        {
            gameObject.SetActive(status);
        }
    }
}