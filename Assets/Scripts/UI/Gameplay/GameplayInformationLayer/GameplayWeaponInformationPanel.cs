using TMPro;
using UnityEngine;


namespace Arenar.UI
{
    public class GameplayWeaponInformationPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _weaponClipSizeText;


        public void UpdateWeaponClipSizeInfo(int currentClipSize, int clipSizeMax)
        {
            _weaponClipSizeText.text = currentClipSize + "/" + clipSizeMax;
        }
        
        public void UpdateWeaponClipSizeInfoPercent(int currentClipSize, int clipSizeMax)
        {
            _weaponClipSizeText.text = ((int)(currentClipSize/clipSizeMax * 100)) + "/" + 100;
        }
    }
}