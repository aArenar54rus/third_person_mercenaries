using UnityEngine;
using UnityEngine.UI;


namespace Arenar
{
    public class AudioButton : Button
    {
        [SerializeField] private Image _soundIcon;
        [SerializeField] private Image _soundCross;


        public void SetSoundStatus(bool status)
        {
            if (status)
            {
                _soundIcon.color = Color.white;
                _soundCross.gameObject.SetActive(false);
            }
            else
            {
                _soundIcon.color = Color.gray;
                _soundCross.gameObject.SetActive(true);
            }
        }
    }
}