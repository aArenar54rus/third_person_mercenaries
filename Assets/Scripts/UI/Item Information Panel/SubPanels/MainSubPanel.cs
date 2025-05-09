using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar
{
    public abstract class MainSubPanel : SubPanel
    {   
        [SerializeField] private TMP_Text itemNameText = default;
        [SerializeField] private TMP_Text itemTypeText = default;
        [SerializeField] private TMP_Text itemDescriptionText = default;
        [SerializeField] private Image icon = default;
        
        
        public override void Initialize(ItemData itemData)
        {
            itemNameText.text = itemData.NameKey;
            itemDescriptionText.text = itemData.DescKey;
            icon.sprite = itemData.Icon;
        }
    }
}