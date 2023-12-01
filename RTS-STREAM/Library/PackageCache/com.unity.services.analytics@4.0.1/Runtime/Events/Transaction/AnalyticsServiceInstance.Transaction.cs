using System;
using UnityEngine;

namespace Unity.Services.Analytics
{
    partial class AnalyticsServiceInstance
    {
        readonly TransactionCurrencyConverter converter = new TransactionCurrencyConverter();

        public void Transaction(TransactionParameters transactionParameters)
        {
            if (!ServiceEnabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(transactionParameters.TransactionName))
            {
                Debug.LogError("Required to have a value for transactionName");
            }

            if (transactionParameters.TransactionType.Equals(TransactionType.INVALID))
            {
                Debug.LogError("Required to have a value for transactionType");
            }

            // If The paymentCountry is not provided we will generate it.

            if (string.IsNullOrEmpty(transactionParameters.PaymentCountry))
            {
                transactionParameters.PaymentCountry = Internal.Platform.UserCountry.Name();
            }

            dataGenerator.Transaction(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.events.transaction", transactionParameters);
        }

        public long ConvertCurrencyToMinorUnits(string currencyCode, double value)
        {
            return converter.Convert(currencyCode, value);
        }
    }
}
