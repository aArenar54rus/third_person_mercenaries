using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.UI
{
    public class ProgressBarController : MonoBehaviour
    {
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TMP_Text _progressText;


        public void SetProgressBarActive(bool status) =>
            _progressSlider.gameObject.SetActive(status);
        
        public void SetBarValue(float value, float valueMax) =>
            SetBarValue(value, valueMax, 0);

        public void SetBarValue(float value, float valueMax, int fractionOrder)
        {
            _progressSlider.maxValue = valueMax;
            _progressSlider.value = value;
            _progressText.text = Math.Round((double)value, fractionOrder) + "/" + Math.Round((double)fractionOrder, fractionOrder);
        }

        public void SetBarValuePercent(float value, float valueMax)
        {
            int progress = (int)(value / valueMax * 100);
            _progressSlider.maxValue = 100;
            _progressSlider.value = progress;
            _progressText.text = progress + "/" + 100;
        }
    }
}