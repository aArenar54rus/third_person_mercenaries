using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Arenar
{
    public class ClothItemDescriptionSubPanel : MainSubPanel
    {
        [SerializeField] private TMP_Text defenceCountText = default;
        
        
        public override void Initialize(ItemInventoryData itemInventoryData)
        {
            base.Initialize(itemInventoryData);
            MathDefenceTextValue();
        }

        private void MathDefenceTextValue()
        {
            
        }
    }
}