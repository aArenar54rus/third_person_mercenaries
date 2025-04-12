using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar.Character
{
    [CreateAssetMenu(menuName = "Characters/Player Character Upgrade Data")]
    public class CharacterParametersUpgradeData : ScriptableObject
    {
        [SerializeField]
        private ParameterProgress[] parameterProgressesByLevel;


        public ParameterProgress GetParameterProgressByType(CharacterSkillUpgradeType skillUpgradeType)
        {
            foreach (var progress in parameterProgressesByLevel)
            {
                if (progress.SkillType == skillUpgradeType)
                    return progress;
            }
            
            Debug.LogError("Not found type: " + skillUpgradeType);
            return null;
        }
        
        

        [Serializable]
        public class ParameterProgress
        {
            [FormerlySerializedAs("parameterType"),SerializeField]
            private CharacterSkillUpgradeType skillType;
            [SerializeField]
            private int[] parameterByLevel;


            public CharacterSkillUpgradeType SkillType => skillType;
            public int[] ParameterByLevel => parameterByLevel;
        }
    }
}