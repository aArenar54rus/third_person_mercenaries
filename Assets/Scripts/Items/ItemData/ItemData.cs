using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(menuName = "Items/ItemData")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] protected int _id = default;
        [SerializeField] protected string _nameKey = default;
        [SerializeField] protected string _desckey = default;
        [SerializeField] protected ItemType _itemType = default;
        [SerializeField] private int _stackCountMax = default;
        [SerializeField] private float _itemMass = default;
        [SerializeField] protected ItemWorldVisual _worldVisual = default;


        public int Id => _id;

        public string NameKey => _nameKey;

        public string DescKey => _desckey;

        public int StackCountMax => _stackCountMax;

        public float ItemMass => _itemMass;

        public bool CanStack =>
            _stackCountMax > 1;

        public ItemType ItemType => _itemType;

        public Sprite Icon =>
            Resources.Load<Sprite>("Sprites/Items/" + _id);

        public ItemWorldVisual WorldVisual => _worldVisual;
    }
}