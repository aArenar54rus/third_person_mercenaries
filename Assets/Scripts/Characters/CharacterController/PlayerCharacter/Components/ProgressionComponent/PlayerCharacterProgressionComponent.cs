using System;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterProgressionComponent : ICharacterProgressionComponent
    {
        private const float FORMULA_ELEMENT_X = 0.2f;
        private const float FORMULA_ELEMENT_Y = 3.0f;
        
        
        public event Action<int, int> OnUpdateExperience;
        public event Action<int> OnUpdateLevel;
        
        
        private int _level;
        private int _experienceMax;
        private int _experience;

        
        public int Level => _level;

        public int ExperienceMax => _experienceMax;
        
        public int Experience 
        { 
            get => _experience;
            set
            {
                _experience = value;
                return;
                if (_experience >= _experienceMax)
                {
                    _experience -= _experienceMax;
                    if (_experience < 0)
                        _experience = 0;
                    _experienceMax = GetExperienceMaxValue();
                    _level++;
                    OnUpdateLevel?.Invoke(_level);
                }
                
                OnUpdateExperience?.Invoke(_experience, _experienceMax);
            }
        }


        [Inject]
        public void Construct()
        {
            
        }

        public void Initialize()
        {
            _level = 0;
            Experience = 0;
        }

        public void DeInitialize() { }

        public void OnActivate() { }
        
        public void OnDeactivate() { }

        private int GetExperienceMaxValue()
        {
            return (int)(((Level + 2) / FORMULA_ELEMENT_X) * FORMULA_ELEMENT_Y);
        }
    }
}