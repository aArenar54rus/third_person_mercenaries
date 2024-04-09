using System.Collections.Generic;
using UnityEngine;

namespace Arenar.Character
{
    [CreateAssetMenu(menuName = "Characters/PlayerCharacterLevelData")]
    public class PlayerCharacterLevelData : ScriptableObject
    {
        [SerializeField] private List<int> _levelsExperience;


        public int MaxLevel => _levelsExperience.Count;

        public int GetExperienceForNextLevel(int currentLevel) =>
            _levelsExperience[currentLevel + 1];
    }
}