using UnityEngine;


namespace Arenar.Services.InventoryService
{
    public class InventoryOptionsSOData : ScriptableObject
    {
        [SerializeField] private int defaultMassMax = default;
        [SerializeField] private int defaultInventoryCellsCount = default;


        public int DefaultMassMax => defaultMassMax;
        public int DefaultInventoryCellsCount => defaultInventoryCellsCount;
    }
}