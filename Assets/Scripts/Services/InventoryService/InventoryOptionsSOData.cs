using System;
using UnityEngine;
using UnityEngine.Serialization;
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
            [SerializeField] private ConstantWeaponCellParameters[] _constantWeaponCellParametersArray;
            [SerializeField] private StarterBagItemCellParameters[] _bagCellParametersArray;


            public int DefaultMassMax => defaultMassMax;
            public int DefaultInventoryCellsCount => defaultInventoryCellsCount;
            public int EquippedWeaponsCount => equippedWeaponsCount;
            public ConstantWeaponCellParameters[] ConstantWeaponCellParametersArray =>
                _constantWeaponCellParametersArray;
            public StarterBagItemCellParameters[] BagCellParametersArray =>
                _bagCellParametersArray;


            [Serializable]
            public class ConstantWeaponCellParameters
            {
                [SerializeField] private int _weaponCellIndex;
                [SerializeField] private bool _isLockWeapon;
                [SerializeField] private ItemInventoryData constantWeaponInventoryData;


                public int WeaponCellIndex => _weaponCellIndex;
                public bool IsLockWeaponCell => _isLockWeapon;
                public ItemInventoryData  ConstantWeaponInventoryData => constantWeaponInventoryData;
            }

            [Serializable]
            public class StarterBagItemCellParameters
            {
                [FormerlySerializedAs("_weaponCellIndex")] [SerializeField] private int bagCellIndex;
                [FormerlySerializedAs("constantWeaponInventoryData")] [SerializeField] private ItemInventoryData bagInventoryData;


                public int BagCellIndex => bagCellIndex;
                public ItemInventoryData  BagInventoryData => bagInventoryData;
            }
        }
    }
}