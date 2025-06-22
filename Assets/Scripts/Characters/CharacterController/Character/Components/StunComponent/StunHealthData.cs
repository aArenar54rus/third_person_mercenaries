using UnityEngine;

namespace Arenar.Character
{
    public class StunHealthData 
    {
        private int _stunHealth;
        private int _stunHealthMax;


        public int StunHealth {
            get => _stunHealth;
            set => _stunHealth = Mathf.Clamp(value, 0, _stunHealthMax);
        }

        public int StunHealthMax => _stunHealthMax;


        public StunHealthData(int startValue) {
            _stunHealthMax = startValue;
            _stunHealth = startValue;
        }
    }
}