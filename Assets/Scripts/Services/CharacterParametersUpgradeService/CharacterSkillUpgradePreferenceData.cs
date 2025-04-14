using Arenar.Character;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Arenar.PreferenceSystem
{
    [Serializable]
    public class CharacterSkillUpgradePreferenceData
    {
        [JsonProperty]
        public int upgradeScore = 0;

        [JsonProperty]
        public Dictionary<CharacterSkillUpgradeType, int> characterSkillUpgradeData = new Dictionary<CharacterSkillUpgradeType, int>()
        {
            { CharacterSkillUpgradeType.HealthMax, -1 },
            { CharacterSkillUpgradeType.DamageMax, -1 },
            { CharacterSkillUpgradeType.MovementSpeedMax, -1 },
        };
    }
}