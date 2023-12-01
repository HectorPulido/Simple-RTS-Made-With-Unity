using System;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Analytics
{
    public class AnalyticsEventHandler : MonoBehaviour
    {
        // Console Log visualiser, not required
        [SerializeField] Text consoleOutput;
        [SerializeField] ScrollRect consoleScrollRect;

        void Awake()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (consoleOutput == null)
                return;

            consoleOutput.text += $"{type}: {condition}\n";
            consoleScrollRect.normalizedPosition = Vector2.zero;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
        
        // Analytics Sample
        async void Start()
        {
            await UnityServices.InitializeAsync();
            await AnalyticsService.Instance.CheckForRequiredConsents();
        }

        public void RecordMinimalAdImpressionEvent()
        {
            StandardEventSample.RecordMinimalAdImpressionEvent();
            Debug.Log("Record Minimal Ad Impression Event Finished");
        }

        public void RecordCompleteAdImpressionEvent()
        {
            StandardEventSample.RecordCompleteAdImpressionEvent();
            Debug.Log("Record Complete Ad Impression Event Finished");
        }

        public void RecordSaleTransactionWithOnlyRequiredValues()
        {
            StandardEventSample.RecordSaleTransactionWithOnlyRequiredValues();
            Debug.Log("Record Sale Transaction With Only Required Values Finished");
        }

        public void RecordSaleTransactionWithRealCurrency()
        {
            StandardEventSample.RecordSaleTransactionWithRealCurrency();
            Debug.Log("Record Sale Transaction With Real Currency Finished");
        }

        public void RecordSaleTransactionWithVirtualCurrency()
        {
            StandardEventSample.RecordSaleTransactionWithVirtualCurrency();
            Debug.Log("Record Sale Transaction With Virtual Currency Finished");
        }

        public void RecordSaleTransactionWithMultipleVirtualCurrencies()
        {
            StandardEventSample.RecordSaleTransactionWithMultipleVirtualCurrencies();
            Debug.Log("Record Sale Transaction With Multiple Virtual Currencies Finished");
        }

        public void RecordSaleEventWithOneItem()
        {
            StandardEventSample.RecordSaleEventWithOneItem();
            Debug.Log("Record Sale Event With One Item Finished");
        }

        public void RecordSaleEventWithMultipleItems()
        {
            StandardEventSample.RecordSaleEventWithMultipleItems();
            Debug.Log("Record Sale Event With Multiple Items Finished");
        }

        public void RecordSaleEventWithOptionalParameters()
        {
            StandardEventSample.RecordSaleEventWithOptionalParameters();
            Debug.Log("Record Sale Event With Optional Parameters Finished");
        }

        public void RecordAcquisitionSourceEventWithOnlyRequiredValues()
        {
            StandardEventSample.RecordAcquisitionSourceEventWithOnlyRequiredValues();
            Debug.Log("Record Acquisition Source Event With Only Required Values Finished");
        }

        public void RecordAcquisitionSourceEventWithOptionalParameters()
        {
            StandardEventSample.RecordAcquisitionSourceEventWithOptionalParameters();
            Debug.Log("Record Acquisition Source Event With Optional Parameters Finished");
        }

        public void RecordPurchaseEventWithOneItem()
        {
            StandardEventSample.RecordPurchaseEventWithOneItem();
            Debug.Log("Record Purchase Event With One Item Finished");
        }

        public void RecordPurchaseEventWithMultipleItems()
        {
            StandardEventSample.RecordPurchaseEventWithMultipleItems();
            Debug.Log("Record Purchase Event With Multiple Items Finished");
        }

        public void RecordPurchaseEventWithMultipleCurrencies()
        {
            StandardEventSample.RecordPurchaseEventWithMultipleCurrencies();
            Debug.Log("Record Purchase Event With Multiple Currencies Finished");
        }

        // Custom events require you to set them up on the dashboard before they can be used
        public void RecordCustomEventWithNoParameters()
        {
            CustomEventSample.RecordCustomEventWithNoParameters();
            Debug.Log("Record Custom Event With No Parameters Finished");
        }

        public void RecordCustomEventWithParameters()
        {
            CustomEventSample.RecordCustomEventWithParameters();
            Debug.Log("Record Custom Event With Parameters Finished");
        }
    }
}
