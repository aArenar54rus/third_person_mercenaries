using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar
{
   public class ItemInteractableElement : InteractableElement
   {
      [SerializeField] private BoxCollider boxCollider = default;
      [SerializeField] private Rigidbody rigidbody = default;
      [SerializeField] private MeshFilter meshFilter = default;
      [SerializeField] private MeshRenderer meshRenderer = default;

      [FormerlySerializedAs("itemInventoryData"),SerializeField] private ItemData itemData;
      [SerializeField] private int _count;
      private bool isInitialized = false;


      public ItemData ItemData => itemData;
      public int Count => _count;


      public override string InteractableElementType
      {
         get
         {
            if (isInitialized)
               return _elementType;

            return null;
         }
      }


      private void Start()
      {
         if (itemData != null)
         {
            SetItem(itemData, _count);
         }
         else
         {
            isInitialized = false;
         }
      }

      public void SetItem(ItemData itemData, int count)
      {
         this.itemData = itemData;
         this._count = count;

         UpdateItemObjectVisual();

         isInitialized = true;
      }

      private void UpdateItemObjectVisual()
      {
         rigidbody.mass = itemData.WorldVisual.Mass;
         meshRenderer.materials = itemData.WorldVisual.Materials;
         meshFilter.mesh = itemData.WorldVisual.Mesh;
         // boxCollider.size = meshFilter.mesh.bounds.size;
      }
   }
}