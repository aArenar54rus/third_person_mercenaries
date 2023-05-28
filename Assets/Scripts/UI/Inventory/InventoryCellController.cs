using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryCellController : MonoBehaviour
{
    [Header("Cell")]
    [SerializeField] private Image containerImage = default;
    [SerializeField] private Image iconImage = default;
    [SerializeField] private TMP_Text countText = default;


    public InventoryItemData CurrentInventoryItemData { get; private set; }
    
    public bool CanAddToCurrentItem =>
        CurrentInventoryItemData.itemData.CanStack
        && CurrentInventoryItemData.elementsCount < CurrentInventoryItemData.itemData.StackCountMax;
    

    public void AddItem(InventoryItemData inventoryItemData)
    {
        containerImage.raycastTarget = true;
        
        iconImage.enabled = true;
        iconImage.sprite = inventoryItemData.itemData.Icon;

        countText.enabled = inventoryItemData.itemData.CanStack;
        countText.text = $"{inventoryItemData.elementsCount}/{inventoryItemData.itemData.StackCountMax}";

        this.CurrentInventoryItemData = inventoryItemData;
    }

    public void TryAddCountToCurrentItem(InventoryItemData addedInventoryItemData, out InventoryItemData returnedInventoryItemData)
    {
        if (CurrentInventoryItemData == null)
        {
            AddItem(addedInventoryItemData);
            returnedInventoryItemData = null;
            return;
        }

        if (!CanAddToCurrentItem)
        {
            returnedInventoryItemData = addedInventoryItemData;
            return;
        }

        CurrentInventoryItemData.elementsCount += addedInventoryItemData.elementsCount;
        if (CurrentInventoryItemData.elementsCount > CurrentInventoryItemData.itemData.StackCountMax)
        {
            returnedInventoryItemData = new InventoryItemData(CurrentInventoryItemData.itemData, 
                CurrentInventoryItemData.elementsCount - CurrentInventoryItemData.itemData.StackCountMax);
        }
        else
        {
            returnedInventoryItemData = null;
        }
    }

    public void ReplaceItem(InventoryItemData addedInventoryItemData, out InventoryItemData returnedInventoryItemData)
    {
        if (CurrentInventoryItemData != null
            && IsSameItem(addedInventoryItemData.itemData))
        {
            TryAddCountToCurrentItem(addedInventoryItemData, out returnedInventoryItemData);
            return;
        }
        
        returnedInventoryItemData = CurrentInventoryItemData ?? null;
        AddItem(addedInventoryItemData);
    }

    public void DropItem()
    {
        containerImage.raycastTarget = false;
        iconImage.enabled = false;
        countText.enabled = false;
        
        this.CurrentInventoryItemData = null;
    }
    
    public bool IsSameItem(ItemData addedItemData) =>
        addedItemData.Id == CurrentInventoryItemData.itemData.Id;
}
