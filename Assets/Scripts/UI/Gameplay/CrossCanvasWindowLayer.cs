using Arenar.Services.UI;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.UI
{
    public class CrossCanvasWindowLayer : CanvasWindowLayer
    {
        [SerializeField] private Image crossImage;
        [SerializeField] private RectTransform crossRT;


        public Image CrossImage => crossImage;
        public RectTransform CrossRT => crossRT;
    }
}