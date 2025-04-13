using System.Collections.Generic;
using Zenject;


namespace Arenar.Character
{
    public class AiCharacterAggressionWithPlayerReactionComponent : ICharacterAggressionComponent
    {
        private const int ONE_HUNDRED = 100;
        
        
        private CharacterSpawnController characterSpawnController;
        private List<AggressionTarget> _characterTargetsData;
        
        
        public ICharacterEntity MaxAggressionTarget { get; private set; }



        [Inject]
        private void Construct(CharacterSpawnController characterSpawnController)
        {
            this.characterSpawnController = characterSpawnController;
        }
        
        public void Initialize()
        {
            _characterTargetsData = new List<AggressionTarget>();
            UpdateAggressionTargets();
        }

        public void DeInitialize()
        {
            _characterTargetsData = null;
        }

        public void OnActivate()
        {
            MaxAggressionTarget = null;
            _characterTargetsData.Clear();
            
            if (characterSpawnController.PlayerCharacter != null)
            {
                AddAggressionScore(characterSpawnController.PlayerCharacter, ONE_HUNDRED);
            }
            else
            {
                characterSpawnController.OnCreatePlayerCharacter += AddPlayerAggressionHandler;
                UpdateAggressionTargets();
            }
        }

        public void OnDeactivate()
        {
            _characterTargetsData.Clear();
            characterSpawnController.OnCreatePlayerCharacter -= AddPlayerAggressionHandler;
        }

        public void AddAggressionScore(ICharacterEntity aggressor, int aggrScore)
        {
            foreach (var aggressionTarget in _characterTargetsData)
            {
                if (aggressionTarget.target == aggressor)
                {
                    aggressionTarget.aggression += aggrScore;
                    UpdateAggressionTargets();
                    return;
                }
            }

            AggressionTarget newTarget = new AggressionTarget();
            newTarget.target = aggressor;
            newTarget.aggression = aggrScore;
            _characterTargetsData.Add(newTarget);
            UpdateAggressionTargets();
        }

        private void UpdateAggressionTargets()
        {
            if (_characterTargetsData.Count == 0)
            {
                MaxAggressionTarget = null;
                return;
            }
            
            ICharacterEntity maxAggressionTarget = null;
            List<AggressionTarget> lostTargets = new();
            int score = 0;
            foreach (var aggressionTarget in _characterTargetsData)
            {
                if (!aggressionTarget.target.TryGetCharacterComponent(out ICharacterLiveComponent liveComponent))
                    continue;

                if (!liveComponent.IsAlive || aggressionTarget.aggression <= 0)
                {
                    lostTargets.Add(aggressionTarget);
                    continue;
                }
                    
                if (aggressionTarget.aggression > score)
                {
                    score = aggressionTarget.aggression;
                    maxAggressionTarget = aggressionTarget.target;
                }
            }

            MaxAggressionTarget = maxAggressionTarget;

            foreach (var lostTarget in lostTargets)
                _characterTargetsData.Remove(lostTarget);
        }
        
        private void AddPlayerAggressionHandler(ICharacterEntity aggressor)
        {
            characterSpawnController.OnCreatePlayerCharacter -= AddPlayerAggressionHandler;

            AddAggressionScore(aggressor, ONE_HUNDRED);
        }
        


        class AggressionTarget
        {
            public int aggression;
            public ICharacterEntity target;
        }
    }
}