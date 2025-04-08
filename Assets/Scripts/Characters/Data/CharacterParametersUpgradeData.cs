using System;
using UnityEngine;


namespace Arenar.Character
{
    [CreateAssetMenu(menuName = "Characters/Player Character Upgrade Data")]
    public class CharacterParametersUpgradeData : ScriptableObject
    {
        [SerializeField]
        private ParameterProgress[] parameterProgressesByLevel;


        public ParameterProgress GetParameterProgressByType(CharacterParameterUpgradeType parameterUpgradeType)
        {
            foreach (var progress in parameterProgressesByLevel)
            {
                if (progress.ParameterType == parameterUpgradeType)
                    return progress;
            }
            
            Debug.LogError("Not found type: " + parameterUpgradeType);
            return null;
        }
        
        

        [Serializable]
        public class ParameterProgress
        {
            [SerializeField]
            private CharacterParameterUpgradeType parameterType;
            [SerializeField]
            private int[] parameterByLevel;


            public CharacterParameterUpgradeType ParameterType => parameterType;
            public int[] ParameterByLevel => parameterByLevel;
        }
    }
}