using System;
using UnityEngine;

namespace Unity.Services.Analytics
{
    partial class AnalyticsServiceInstance
    {
        public void TransactionFailed(TransactionFailedParameters parameters)
        {
            if (!ServiceEnabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(parameters.TransactionName))
            {
                Debug.LogError("Required to have a value for transactionName");
            }

            if (parameters.TransactionType.Equals(TransactionType.INVALID))
            {
                Debug.LogError("Required to have a value for transactionType");
            }

            if (string.IsNullOrEmpty(parameters.FailureReason))
            {
                Debug.LogError("Required to have a failure reason in transactionFailed event");
            }

            if (string.IsNullOrEmpty(parameters.PaymentCountry))
            {
                parameters.PaymentCountry = Internal.Platform.UserCountry.Name();
            }

            dataGenerator.TransactionFailed(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.events.TransactionFailed", parameters);
        }
    }
}
