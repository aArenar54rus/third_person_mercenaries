using Arenar.UI;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class InventoryControlButtonsCanvasLayer : CanvasWindowLayer
    {
        [SerializeField]
        private Button _backButton;
        [SerializeField]
        private Button debugButton;
        [SerializeField]
        private MoneyWallet moneyWallet;


        public Button BackButton => _backButton;
        public Button DebugButton => debugButton;
        public MoneyWallet MoneyWallet => moneyWallet;
    }
}