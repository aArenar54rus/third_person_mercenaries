using System;
using UnityEngine;


namespace Arenar.Character
{
    [Serializable]
    public class CharacterAudioDataStorage
    {
        [SerializeField] private AudioSource audioSource = default;
        [SerializeField] private AnimationReactionsController animationReactionsController = default;


        public AudioSource AudioSource => audioSource;
        public AnimationReactionsController AnimationReactionsController => animationReactionsController;
    }
}