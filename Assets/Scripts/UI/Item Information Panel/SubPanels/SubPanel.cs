using Arenar;
using UnityEngine;


public abstract class SubPanel : MonoBehaviour
{
    private RectTransform _rectTransform;


    public RectTransform RectTransform => 
        _rectTransform ??= GetComponent<RectTransform>();
    
    
    public abstract void Initialize(ItemData itemData);
}
