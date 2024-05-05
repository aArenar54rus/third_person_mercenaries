using System.Collections.Generic;


namespace Arenar.Character
{
    public class AiSearchTargetComponent : ISearchTargetComponent
    {
        private List<AggressionTarget> _characterTargets;


        public ICharacterEntity CharacterEntityTarget { get; private set; }
        
        
        
        public void AddAggression(int aggression, ICharacterEntity aggressionCharacter)
        {
            foreach (var target in _characterTargets)
            {
                if (aggressionCharacter == target.target)
                {
                    target.aggression += aggression;
                    if (target.aggression <= 0)
                        _characterTargets.Remove(target);
                    ChangeTarget();
                    return;
                }
            }

            if (aggression <= 0)
                return;
            
            AggressionTarget newTarget = new AggressionTarget();
            newTarget.aggression = aggression;
            newTarget.target = aggressionCharacter;
            _characterTargets.Add(newTarget);
            ChangeTarget();
        }

        public void Initialize()
        {
            _characterTargets = new List<AggressionTarget>();
            CharacterEntityTarget = null;
        }

        public void DeInitialize()
        {
            CharacterEntityTarget = null;
            _characterTargets.Clear();
            _characterTargets = null;
        }

        public void OnActivate()
        {
            _characterTargets.Clear();
            CharacterEntityTarget = null;
        }

        public void OnDeactivate()
        {
            CharacterEntityTarget = null;
        }

        private void ChangeTarget()
        {
            if (_characterTargets == null || _characterTargets.Count == 0)
            {
                CharacterEntityTarget = null;
                return;
            }
            
            int aggressionMax = 0;
            foreach (var target in _characterTargets)
            {
                if (target.aggression > aggressionMax)
                {
                    aggressionMax = target.aggression;
                    CharacterEntityTarget = target.target;
                }
            }
        }


        class AggressionTarget
        {
            public int aggression;
            public ICharacterEntity target;
        }
    }
}