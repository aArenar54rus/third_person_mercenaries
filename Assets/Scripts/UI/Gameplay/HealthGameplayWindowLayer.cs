using Arenar.Services.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.UI
{
    public class HealthGameplayWindowLayer : CanvasWindowLayer
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Slider healthSlider;


        public TMP_Text HealthText => healthText;
        public Slider HealthSlider => healthSlider;
    }
}