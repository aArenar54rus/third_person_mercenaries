using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Arenar
{
    public class ItemParametersSubPanel : SubPanel
    {
        [SerializeField] private TMP_Text textPrefab = default;

        private List<TMP_Text> parameterTexts = default;
        
        
        public override void Initialize(ItemInventoryData itemInventoryData)
        {
            ClearOldTexts();
        }

        private void ClearOldTexts()
        {
            if (parameterTexts == null
                || parameterTexts.Count == 0)
            {
                parameterTexts = new List<TMP_Text>();
                return;
            }

            for (int i = parameterTexts.Count - 1; i >= 0; i--)
                Destroy(parameterTexts[i]);
            
            parameterTexts.Clear();
        }
    }
}