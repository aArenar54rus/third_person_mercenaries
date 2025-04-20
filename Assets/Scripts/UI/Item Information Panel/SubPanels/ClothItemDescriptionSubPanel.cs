using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Arenar
{
    public class ClothItemDescriptionSubPanel : MainSubPanel
    {
        [SerializeField] private TMP_Text defenceCountText = default;
        
        
        public override void Initialize(ItemData itemData)
        {
            base.Initialize(itemData);
            MathDefenceTextValue();
        }

        private void MathDefenceTextValue()
        {
            
        }
    }
}