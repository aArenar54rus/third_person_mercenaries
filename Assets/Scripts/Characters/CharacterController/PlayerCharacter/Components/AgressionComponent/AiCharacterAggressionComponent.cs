using System.Collections.Generic;


namespace Arenar.Character
{
    public class AiCharacterAggressionComponent : ICharacterAggressionComponent
    {
        public Dictionary<ICharacterEntity, int> CharacterAggressionScores { get; private set; }
        public ICharacterEntity AggressionTarget { get; private set; }
        
        
        public void Initialize()
        {
            CharacterAggressionScores = new Dictionary<ICharacterEntity, int>();
            UpdateAggressionTargets();
        }

        public void DeInitialize()
        {
            CharacterAggressionScores.Clear();
            CharacterAggressionScores = null;
        }

        public void OnActivate()
        {
            CharacterAggressionScores.Clear();
            UpdateAggressionTargets();
        }

        public void OnDeactivate()
        {
            CharacterAggressionScores.Clear();
        }

        public void AddAggressionScore(ICharacterEntity aggressor, int aggrScore)
        {
            if (!CharacterAggressionScores.ContainsKey(aggressor))
            {
                CharacterAggressionScores.Add(aggressor, aggrScore);
                return;
            }

            CharacterAggressionScores[aggressor] += aggrScore;
            UpdateAggressionTargets();
        }

        private void UpdateAggressionTargets()
        {
            if (CharacterAggressionScores.Count == 0)
            {
                AggressionTarget = null;
                return;
            }
            
            ICharacterEntity newAggressionTarget = null;
            List<ICharacterEntity> lostTargets = new();
            int score = 0;
            foreach (var aggression in CharacterAggressionScores)
            {
                if (!aggression.Key.TryGetCharacterComponent(out ICharacterLiveComponent liveComponent))
                    continue;

                if (!liveComponent.IsAlive || aggression.Value <= 0)
                {
                    lostTargets.Add(aggression.Key);
                    continue;
                }
                    
                if (aggression.Value > score)
                {
                    score = aggression.Value;
                    newAggressionTarget = aggression.Key;
                }
            }

            AggressionTarget = newAggressionTarget;

            foreach (var lostTarget in lostTargets)
                CharacterAggressionScores.Remove(lostTarget);
        }
    }
}