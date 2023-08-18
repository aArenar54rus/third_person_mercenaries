using UnityEngine;


public class CharacterPhysicsInteraction : MonoBehaviour
{
    [SerializeField] private Transform characterTransform;
    [SerializeField] private float forceMagnitude;

    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;

        if (rigidbody == null)
            return;

        Vector3 forceDirection = hit.gameObject.transform.position - characterTransform.position;
        forceDirection.y = 0;
        forceDirection.Normalize();
        
        rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, characterTransform.position, ForceMode.Impulse);
    }
}
