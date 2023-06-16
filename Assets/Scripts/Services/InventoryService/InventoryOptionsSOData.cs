using System;
using UnityEngine;
using Zenject;


namespace Arenar.Services.InventoryService
{
    [CreateAssetMenu(fileName = "Inventory Data", menuName = "Data/Inventory Data", order = 1)]
    public class InventoryOptionsSOData : ScriptableObjectInstaller<InventoryOptionsSOData>
    {
        [SerializeField] private Parameters _parameters;


        public override void InstallBindings()
        {
            
        }


        [Serializable]
        public class Parameters
        {
            [SerializeField] private int defaultMassMax = default;
            [SerializeField] private int defaultInventoryCellsCount = default;


            public int DefaultMassMax => defaultMassMax;
            public int DefaultInventoryCellsCount => defaultInventoryCellsCount;
        }
    }
}