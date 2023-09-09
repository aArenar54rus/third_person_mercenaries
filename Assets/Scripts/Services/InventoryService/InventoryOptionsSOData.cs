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
            Container.Bind<Parameters>().FromInstance(_parameters).AsSingle();
        }


        [Serializable]
        public class Parameters
        {
            [SerializeField] private int defaultMassMax = default;
            [SerializeField] private int defaultInventoryCellsCount = default;
            [SerializeField] private int equippedWeaponsCount = 4;


            public int DefaultMassMax => defaultMassMax;
            public int DefaultInventoryCellsCount => defaultInventoryCellsCount;
            public int EquippedWeaponsCount => equippedWeaponsCount;
        }
    }
}