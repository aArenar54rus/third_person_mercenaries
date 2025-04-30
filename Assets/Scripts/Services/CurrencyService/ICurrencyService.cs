using System;
using System.Collections.Generic;

namespace Arenar.Services
{
	public interface ICurrencyService
	{
		Action<CurrencyType> OnCurrencyValueChange { get; set; }

		ICollection<float> GetAllTransactions(CurrencyType currencyType);
		float GetLastTransaction(CurrencyType currencyType);
		void AddCurrencyValue(params (CurrencyType, float)[] values);
		void AddCurrencyValue(params (CurrencyType, float, float)[] values);
		void SubtractCurrencyValue(params (CurrencyType, float)[] values);
		void AddCurrency(params ICurrency[] values);
		void SubtractCurrency(params ICurrency[] values);
		bool TrySubtractCurrency(params ICurrency[] values);
		void SetCurrencyValue(params (CurrencyType, float)[] values);
		bool TrySubtractCurrencyValue(params (CurrencyType, float)[] values);
		float GetCurrencyValue(CurrencyType currencyType);
		float GetCurrencyValueMax(CurrencyType currencyType);
		ICurrency GetCurrency(CurrencyType currencyType);
		bool IsEnoughCurrency(CurrencyType currencyType, float value);
	}
}
