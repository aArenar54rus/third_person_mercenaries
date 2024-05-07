using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(menuName = "Items/ItemInventoryData")]
    public class ItemInventoryData : ScriptableObject
    {
        [SerializeField] protected int _id = default;
        [SerializeField] protected string _nameKey = default;
        [SerializeField] protected string _desckey = default;
        [SerializeField] protected ItemType _itemType = default;
        [SerializeField] protected ItemRarity _itemRarity = default;
        [SerializeField] private int _stackCountMax = default;
        [SerializeField] private float _itemMass = default;
        
        [Space(10)]
        [SerializeField] private float _bulletPhysicalMight = default;
        [SerializeField] private float _defaultDamage = default;
        [SerializeField] private float _minDamageByLvl = default;
        [SerializeField] private float _maxDamageByLvl = default;
        
        [Space(10)]
        [SerializeField] protected ItemWorldVisual _worldVisual = default;


        public int Id => _id;

        public string NameKey => _nameKey;

        public string DescKey => _desckey;

        public int StackCountMax => _stackCountMax;

        public float ItemMass => _itemMass;

        public float BulletPhysicalMight => _bulletPhysicalMight;
        
        public float DefaultDamage => _defaultDamage;
        public float BulletLevelMinDamage => _minDamageByLvl;
        public float BulletLevelMaxDamage => _maxDamageByLvl;

        public bool CanStack =>
            _stackCountMax > 1;

        public ItemType ItemType => _itemType;
        
        public ItemRarity ItemRarity => _itemRarity;

        public Sprite Icon =>
            Resources.Load<Sprite>("Sprites/Items/" + _id);

        public ItemWorldVisual WorldVisual => _worldVisual;
    }
}