using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class MainMenuPlayerInformationLayer : CanvasWindowLayer
    {
        [SerializeField] private TMP_Text _nickNameText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private RawImage _characterRawImage;


        public TMP_Text NickNameText => _nickNameText;
        public TMP_Text LevelText => _levelText;
    }
}