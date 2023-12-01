using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Unity.Services.Analytics
{
    class TransactionCurrencyConverter
    {
        Dictionary<string, int> m_CurrencyCodeToMinorUnits;

        public long Convert(string currencyCode, double value)
        {
            if (m_CurrencyCodeToMinorUnits == null)
            {
                LoadCurrencyCodeDictionary();
            }

            var currencyCodeUppercased = currencyCode.ToUpperInvariant();
            if (!m_CurrencyCodeToMinorUnits.ContainsKey(currencyCodeUppercased))
            {
                Debug.LogWarning("Unknown currency provided to convert method, no conversion will be performed and returned value will be 0.");
                return 0;
            }

            var numberOfMinorUnits = m_CurrencyCodeToMinorUnits[currencyCode];
            return (long)(value * Math.Pow(10, numberOfMinorUnits));
        }

        public void LoadCurrencyCodeDictionary()
        {
            var text = (Resources.Load("iso4217", typeof(TextAsset)) as TextAsset)?.text;

            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("Error loading currency definitions, no conversions will be performed.");
                m_CurrencyCodeToMinorUnits = new Dictionary<string, int>();
                return;
            }

            try
            {
                m_CurrencyCodeToMinorUnits = JsonConvert.DeserializeObject<Dictionary<string, int>>(text);
            }
            catch (JsonException e)
            {
                Debug.LogWarning($"Failed to deserialize JSON for currency conversion, no conversions will be performed");
                Debug.LogWarning(e.Message);
                m_CurrencyCodeToMinorUnits = new Dictionary<string, int>();
            }
        }
    }
}
