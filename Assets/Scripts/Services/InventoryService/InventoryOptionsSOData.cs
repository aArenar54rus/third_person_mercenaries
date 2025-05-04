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
            [SerializeField]
            private int defaultMassMax = default;
            [SerializeField]
            private int defaultInventoryCellsCount = default;
            [SerializeField]
            private int equippedWeaponsCount = 4;
            [SerializeField]
            private ConstantWeaponCellParameters constantMeleeWeaponCellParameters;
            [SerializeField]
            private ConstantWeaponCellParameters[] constantWeaponCellParametersArray;
            [SerializeField]
            private StarterBagItemCellParameters[] _bagCellParametersArray;

            [Space(5), Header("Mass")]
            [SerializeField]
            private bool isMathMass = false;
            [SerializeField]
            private bool isMathMassOfEquipped = false;
            


            public int DefaultMassMax => defaultMassMax;
            public int DefaultInventoryCellsCount => defaultInventoryCellsCount;
            public int EquippedWeaponsCount => equippedWeaponsCount;
            public ConstantWeaponCellParameters ConstantMeleeWeaponCellParameters =>
                constantMeleeWeaponCellParameters;
            public ConstantWeaponCellParameters[] ConstantWeaponCellParametersArray =>
                constantWeaponCellParametersArray;
            public StarterBagItemCellParameters[] BagCellParametersArray =>
                _bagCellParametersArray;
            public bool IsMathMass => isMathMass;
            public bool IsMathMassEquipped => isMathMassOfEquipped;


            [Serializable]
            public class ConstantWeaponCellParameters
            {
                [SerializeField]
                private int _weaponCellIndex;
                [FormerlySerializedAs("constantWeaponDataIndex"),SerializeField]
                private int constantWeaponId;


                public int WeaponCellIndex => _weaponCellIndex;
                public int ConstantWeaponId => constantWeaponId;
            }

            [Serializable]
            public class StarterBagItemCellParameters
            {
                [SerializeField]
                private int itemBagCellIndex;
                [SerializeField]
                private int itemCount = 1;
                [SerializeField]
                private ItemData bagData;


                public int ItemBagCellIndex => itemBagCellIndex;
                public int ItemCount => itemCount;
                public ItemData  BagData => bagData;
            }
        }
    }
}