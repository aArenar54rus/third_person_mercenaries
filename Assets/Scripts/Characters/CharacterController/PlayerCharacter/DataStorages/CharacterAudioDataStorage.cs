using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar.Character
{
    [Serializable]
    public class CharacterAudioDataStorage
    {
        [SerializeField] private AudioSource audioSource = default;
        [FormerlySerializedAs("animationReactionsController")] [SerializeField] private AnimationReactionsTriggerController animationReactionsTriggerController = default;


        public AudioSource AudioSource => audioSource;
        public AnimationReactionsTriggerController AnimationReactionsTriggerController => animationReactionsTriggerController;
    }
}