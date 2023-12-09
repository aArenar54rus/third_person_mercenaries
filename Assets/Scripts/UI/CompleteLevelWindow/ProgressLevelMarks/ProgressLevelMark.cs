using UnityEngine;
using UnityEngine.UI;


public class ProgressLevelMark : MonoBehaviour
{
    [SerializeField] private Image _markImage;
    [SerializeField] private Sprite _activeImage;
    [SerializeField] private Sprite _deactiveImage;


    public void SetMarkSuccessStatus(bool status) =>
        _markImage.sprite = status ? _activeImage : _deactiveImage;
}
