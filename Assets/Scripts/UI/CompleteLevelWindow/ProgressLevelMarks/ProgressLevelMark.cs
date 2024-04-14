using UnityEngine;
using UnityEngine.UI;

namespace Arenar.Services.UI
{
    public class ProgressLevelMark : MonoBehaviour
    {
        [SerializeField] private Image _markImage;
        [SerializeField] private Sprite _successSprite;
        [SerializeField] private Sprite _failedSprite;


        public void SetMarkSuccessStatus(bool status) =>
            _markImage.sprite = status ? _successSprite : _failedSprite;
    }
}