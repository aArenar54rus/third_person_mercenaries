using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class InventoryCellController : MonoBehaviour
{
    
    [Header("Cell")]
    [FormerlySerializedAs("containerImage")][SerializeField] private Image _containerImage = default;
    [FormerlySerializedAs("iconImage")] [SerializeField] private Image _iconImage = default;
    [FormerlySerializedAs("countText")][SerializeField] private TMP_Text _countText = default;
    
    public int CellIndex { get; private set; }
    
    

    public void Initialize(int cellIndex)
    {
        CellIndex = cellIndex;
    }

    public void SetItem(InventoryItemData inventoryItemData)
    {
        _containerImage.raycastTarget = true;
        
        _iconImage.enabled = true;
        _iconImage.sprite = inventoryItemData.itemData.Icon;

        _countText.enabled = inventoryItemData.itemData.CanStack;
        _countText.text = $"{inventoryItemData.elementsCount}/{inventoryItemData.itemData.StackCountMax}";
    }

    public void SetEmpty()
    {
        _containerImage.raycastTarget = false;
        _iconImage.enabled = false;
        _countText.enabled = false;
    }
}
