using UnityEngine;

namespace Arenar.Items
{
    [CreateAssetMenu(menuName = "Items/Material Item Data")]
    public class MaterialItemData : ItemData
    {
        [SerializeField]
        private int materialMight;


        public int MaterialMight => materialMight;
    }
}