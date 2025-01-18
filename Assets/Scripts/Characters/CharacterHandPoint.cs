using Arenar.Items;
using UnityEditor;
using UnityEngine;


namespace Arenar.Character
{
    public class CharacterHandPoint : MonoBehaviour
    {
        private ICharacterEntity _characterEntity;


        public void Initialize(ICharacterEntity characterEntity)
        {
            _characterEntity = characterEntity;
        }
        
        public void AddItemInHand(IEquippedItem item, Vector3 rotation)
        {
            item.transform.SetParent(this.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}