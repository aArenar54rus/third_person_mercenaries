using System;
using System.Collections;
using System.Collections.Generic;
using Arenar;
using Arenar.Character;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class OverlapSphereTest : MonoBehaviour
{
    [SerializeField] private ComponentCharacterController characterController;
    [SerializeField] private float radius = .28f;
    [SerializeField] private float offset = -0.14f;


    private Transform characterTransform
    {
        get
        {
            if (characterController is PlayerComponentCharacterController player)
                return player.CameraTransform;

            return null;
        }
    }
        

    private ICharacterRayCastComponent characterRaycastComponent;
    

    private void OnDestroy()
    {
        characterRaycastComponent = null;
    }

    private void OnDrawGizmos()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);

        Gizmos.color = transparentGreen;

        Gizmos.DrawSphere(
            new Vector3(characterTransform.position.x, characterTransform.position.y + offset, characterTransform.position.z),
            radius);
    }
}
