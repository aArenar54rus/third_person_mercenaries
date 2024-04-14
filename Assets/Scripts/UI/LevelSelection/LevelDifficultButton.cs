using System;
using Arenar;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelDifficultButton : MonoBehaviour
{
    [SerializeField] private Button _levelDiddicultButton;
    [SerializeField] private Image _levelDiddicultButtonImage;
    [SerializeField] private LevelDifficult levelDifficult;
    [SerializeField] private TMP_Text _levelDifficultText;

    [Space(10), Header("Data")]
    [SerializeField] private Color _enableDifficultColor;
    [SerializeField] private Color _disableDifficultColor;
    [SerializeField] private SerializableDictionary<LevelDifficult, string> _localizationKeys = new()
    {
        {LevelDifficult.Easy, "loc_key_easy"},
        {LevelDifficult.Medium, "loc_key_medium"},
        {LevelDifficult.Hard, "loc_key_hard"},
        {LevelDifficult.UltraHard, "loc_key_ultrahard"},
        {LevelDifficult.Infinity, "loc_key_infinity"},
    };


    public LevelDifficult LevelDifficult => levelDifficult;
    
    public bool Interactable
    {
        get => _levelDiddicultButton.interactable;
        set => _levelDiddicultButton.interactable = value;
    }
    

    public void Initialize(Action<LevelDifficult> action)
    {
        LocalizationManager.OnLocalizeEvent += OnLocalize;
        _levelDiddicultButton.onClick.AddListener(() => action?.Invoke(levelDifficult));
        _levelDiddicultButton.interactable = true;

        OnLocalize();
    }

    public void SetButtonActive(bool status)
    {
        _levelDiddicultButton.interactable = !status;
        _levelDiddicultButtonImage.color = status ? _enableDifficultColor : _disableDifficultColor;
    }

    private void OnLocalize()
    {
        _levelDifficultText.text = LocalizationManager.GetTranslation(_localizationKeys[levelDifficult]);
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLocalizeEvent -= OnLocalize;
        _levelDiddicultButton.onClick.RemoveAllListeners();
    }
}
