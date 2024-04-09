using UnityEngine;
using UnityEngine.UI;

namespace Arenar.Services.UI
{
    public class ProgressLevelMark : MonoBehaviour
    {
        [SerializeField] private Image _markImage;
        [SerializeField] private Color _successColor;
        [SerializeField] private Color _failedColor;


        public void SetMarkSuccessStatus(bool status) =>
            _markImage.color = status ? _successColor : _failedColor;
    }
}