using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class LevelSelectionButtonVisual : MonoBehaviour
    {
        public enum ButtonStatus : byte
        {
            None = 0,
            Selected = 1,
            Active = 2,
            Locked = 3,
        }
        
        
        [SerializeField] private Button _levelSelectionButton;
        [SerializeField] private Image _levelPortraitImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private TMP_Text _levelNameText;
        [SerializeField] private TMP_Text _enemiesLevelsText;

        [Space(10), Header("Parameters")]
        [SerializeField] private string _lockedLevelTextKey;
        [SerializeField] private SerializableDictionary<ButtonStatus, ButtonParameters> _buttonParameters;

        
        private event Action<int> onButtonAction;
        private LevelData _levelData;
        private bool _isInteractable;
        
        
        public ButtonStatus ButtonStatusValue { get; private set; }

        public LevelData LevelData => _levelData;


        public void Initialize(LevelData levelData, Action<int> onButtonAction)
        {
            _levelData = levelData;
            this.onButtonAction = onButtonAction;

            OnLocalize();
            _levelPortraitImage.sprite = levelData.LevelPortrait;
            
            _levelSelectionButton.onClick.AddListener(() =>
            {
                if (!_isInteractable)
                    return;
                
                onButtonAction?.Invoke(_levelData.LevelIndex);
            });
            
            LocalizationManager.OnLocalizeEvent += OnLocalize;
        }

        public void SetDifficultLevel(LevelDifficult difficult)
        {
            LevelData.LevelDifficultData difficultData = _levelData.DifficultData[difficult];
            _enemiesLevelsText.text = difficultData.MinimalEnemyLevel + "-" + difficultData.MaximumEnemyLevel;
        }

        public void SetButtonStatus(ButtonStatus status)
        {
            ButtonStatusValue = status;
            _isInteractable = (status == ButtonStatus.Active);
            
            var parameters = _buttonParameters[status];
            _levelPortraitImage.color = parameters.buttonColor;
            _frameImage.color = parameters.frameColor;
            
            OnLocalize();
        }

        private void OnLocalize()
        {
            string localize = LocalizationManager.GetTranslation(ButtonStatusValue != ButtonStatus.Locked ? _levelData.LevelNameKey : _lockedLevelTextKey);
            _levelNameText.text = localize;
        }

        private void OnDestroy()
        {
            _levelSelectionButton.onClick.RemoveAllListeners();
            LocalizationManager.OnLocalizeEvent -= OnLocalize;
        }


        [Serializable]
        private class ButtonParameters
        {
            public Color buttonColor;
            public Color frameColor;
        }
    }
}