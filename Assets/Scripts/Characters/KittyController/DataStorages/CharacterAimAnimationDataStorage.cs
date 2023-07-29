using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;


namespace Arenar.Character
{
    [Serializable]
    public class CharacterAimAnimationDataStorage
    {
        [SerializeField] private Rig headRig;
        [SerializeField] private Rig bodyRig;
        [SerializeField] private Transform headAimPointObject = default;
        [SerializeField] private Transform bodyAimPointObject = default;


        public Rig HeadRig => headRig;
        public Rig BodyRig => bodyRig;
        public Transform HeadAimPointObject => headAimPointObject;
        public Transform BodyAimPointObject => bodyAimPointObject;
    }
}