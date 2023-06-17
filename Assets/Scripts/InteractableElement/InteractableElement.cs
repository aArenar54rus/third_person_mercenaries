using UnityEngine;


namespace Arenar
{
    public abstract class InteractableElement : MonoBehaviour
    {
        [SerializeField] protected string _elementType = default;


        public virtual string InteractableElementType => _elementType;
    }
}