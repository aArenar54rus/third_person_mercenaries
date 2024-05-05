using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.UI
{
    public class GameplayWeaponInformationPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _weaponClipSizeText;
        [SerializeField] private Slider _weaponClipSizeSlider;


        public void UpdateWeaponClipSizeInfo(int currentClipSize, int clipSizeMax)
        {
            _weaponClipSizeText.text = currentClipSize + "/" + clipSizeMax;
            _weaponClipSizeSlider.maxValue = clipSizeMax;
            _weaponClipSizeSlider.value = currentClipSize;
        }
        
        public void UpdateWeaponClipSizeInfoPercent(int currentClipSize, int clipSizeMax)
        {
            _weaponClipSizeText.text = ((int)(currentClipSize/clipSizeMax * 100)) + "/" + 100;
            _weaponClipSizeSlider.maxValue = clipSizeMax;
            _weaponClipSizeSlider.value = currentClipSize;
        }
    }
}