using UnityEngine;


public class ItemWorldObjectControl : MonoBehaviour
{
   [SerializeField] private BoxCollider boxCollider = default;
   [SerializeField] private Rigidbody rigidbody = default;
   [SerializeField] private MeshFilter meshFilter = default;
   [SerializeField] private MeshRenderer meshRenderer = default;
   
   private ItemData itemData;
   
   
   public void Initialize(ItemData itemData)
   {
      this.itemData = itemData;

      UpdateItemObjectVisual();
   }

   private void UpdateItemObjectVisual()
   {
      rigidbody.mass = itemData.WorldVisuals[0].Mass;
      meshRenderer.materials = itemData.WorldVisuals[0].Materials;
      meshFilter.mesh = itemData.WorldVisuals[0].Mesh;
      boxCollider.size = meshFilter.mesh.bounds.size;
   }
}
