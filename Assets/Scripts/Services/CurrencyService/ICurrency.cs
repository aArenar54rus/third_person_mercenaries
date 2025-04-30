using System;

namespace Arenar.Services
{
    public interface ICurrency
    {
        float Value { get; set; }
        float ValueMax { get; }

        CurrencyType CurrencyType { get; }
		
        
        public event Action<float> OnCurrencyChanged;
    }
}
