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

      [FormerlySerializedAs("itemData")] [SerializeField] private ItemInventoryData itemInventoryData;
      [SerializeField] private int _count;
      private bool isInitialized = false;


      public ItemInventoryData ItemInventoryData => itemInventoryData;
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
         if (itemInventoryData != null)
         {
            SetItem(itemInventoryData, _count);
         }
         else
         {
            isInitialized = false;
         }
      }

      public void SetItem(ItemInventoryData itemInventoryData, int count)
      {
         this.itemInventoryData = itemInventoryData;
         this._count = count;

         UpdateItemObjectVisual();

         isInitialized = true;
      }

      private void UpdateItemObjectVisual()
      {
         rigidbody.mass = itemInventoryData.WorldVisual.Mass;
         meshRenderer.materials = itemInventoryData.WorldVisual.Materials;
         meshFilter.mesh = itemInventoryData.WorldVisual.Mesh;
         // boxCollider.size = meshFilter.mesh.bounds.size;
      }
   }
}