using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class MainMenuButtonsLayer : CanvasWindowLayer
    {
        [SerializeField] private Button _newChallengeButton;
        [SerializeField] private Button _outfitButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _rateUsButton;


        public Button NewChallengeButton => _newChallengeButton;
        public Button OutfitButton => _outfitButton;
        public Button OptionsButton => _optionsButton;
        public Button RateUsButton => _rateUsButton;
    }
}