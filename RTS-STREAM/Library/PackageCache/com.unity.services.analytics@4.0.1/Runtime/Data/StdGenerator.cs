using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Services.Analytics.Tests")]

namespace Unity.Services.Analytics.Data
{
    interface IDataGenerator
    {
        void GameRunning(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier);
        void SdkStartup(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier);
        void NewPlayer(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, string deviceModel);
        void GameStarted(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, string idLocalProject, string osVersion, bool isTiny, bool debugDevice, string userLocale);
        void GameEnded(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, DataGenerator.SessionEndState quitState);
        void AdImpression(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier,
            AdImpressionParameters adImpressionParameters);
        void Transaction(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, TransactionParameters transactionParameters);
        void TransactionFailed(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, TransactionFailedParameters transactionParameters);
        void ClientDevice(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier,
            string cpuType, string gpuType, Int64 cpuCores, Int64 ramTotal, Int64 screenWidth, Int64 screenHeight, Int64 screenDPI);
        void AcquisitionSource(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, AcquisitionSourceParameters acquisitionSourceParameters);
    }

    /// <summary>
    /// DataGenerator is used to push event data into the internal buffer.
    /// The reason its split like this is so we can test the output from
    /// The DataGenerator + InternalBuffer. If this output is validated we
    /// can be pretty confident we are always producing valid JSON for the
    /// backend.
    /// </summary>
    class DataGenerator : IDataGenerator
    {
        public void SdkStartup(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier)
        {
            buf.PushStartEvent("sdkStart", datetime, 1, true);
            buf.PushString(SdkVersion.SDK_VERSION, "sdkVersion");

            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);
            buf.PushString("com.unity.services.analytics", "sdkName"); // Schema: Required

            buf.PushEndEvent();
        }

        public void GameRunning(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier)
        {
            buf.PushStartEvent("gameRunning", datetime, 1, true);

            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);

            buf.PushEndEvent();
        }

        public void NewPlayer(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, string deviceModel)
        {
            buf.PushStartEvent("newPlayer", datetime, 1, true);

            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);
            // We aren't sending deviceBrand at the moment as deviceModel is sufficient.
            // UA1 did not send deviceBrand either. See JIRA-196 for more info.
            buf.PushString(deviceModel, "deviceModel"); // Schema: Optional

            buf.PushEndEvent();
        }

        public void GameStarted(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams,
            string callingMethodIdentifier, string idLocalProject, string osVersion, bool isTiny, bool debugDevice, string userLocale)
        {
            buf.PushStartEvent("gameStarted", datetime, 1, true);

            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);

            // Schema: Required
            buf.PushString(userLocale, "userLocale");

            // Schema: Optional
            if (!String.IsNullOrEmpty(idLocalProject))
            {
                buf.PushString(idLocalProject, "idLocalProject");
            }
            buf.PushString(osVersion, "osVersion");
            buf.PushBool(isTiny, "isTiny");
            buf.PushBool(debugDevice, "debugDevice");

            buf.PushEndEvent();
        }

        // Keep the enum values in Caps!
        // We stringify the values.
        // These values aren't listed as an enum the Schema, but they are listed
        // values here http://go/UA2_Spreadsheet
        internal enum SessionEndState
        {
            PAUSED,
            KILLEDINBACKGROUND,
            KILLEDINFOREGROUND,
            QUIT,
        }

        public void GameEnded(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, SessionEndState quitState)
        {
            buf.PushStartEvent("gameEnded", datetime, 1, true);

            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);

            buf.PushString(quitState.ToString(), "sessionEndState"); // Schema: Required

            buf.PushEndEvent();
        }

        public void AdImpression(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, AdImpressionParameters adImpressionParameters)
        {
            buf.PushStartEvent("adImpression", datetime, 1, true);

            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);

            // Schema: Required

            buf.PushString(adImpressionParameters.AdCompletionStatus.ToString().ToUpperInvariant(), "adCompletionStatus");
            buf.PushString(adImpressionParameters.AdProvider.ToString().ToUpperInvariant(), "adProvider");
            buf.PushString(adImpressionParameters.PlacementID, "placementId");
            buf.PushString(adImpressionParameters.PlacementName, "placementName");

            // Schema: Optional

            if (adImpressionParameters.AdEcpmUsd is double adEcpmUsdValue)
            {
                buf.PushDouble(adEcpmUsdValue, "adEcpmUsd");
            }

            if (adImpressionParameters.PlacementType != null)
            {
                buf.PushString(adImpressionParameters.PlacementType.ToString(), "placementType");
            }

            if (!string.IsNullOrEmpty(adImpressionParameters.SdkVersion))
            {
                buf.PushString(adImpressionParameters.SdkVersion, "adSdkVersion");
            }

            if (!string.IsNullOrEmpty(adImpressionParameters.AdImpressionID))
            {
                buf.PushString(adImpressionParameters.AdImpressionID, "adImpressionID");
            }

            if (!string.IsNullOrEmpty(adImpressionParameters.AdStoreDstID))
            {
                buf.PushString(adImpressionParameters.AdStoreDstID, "adStoreDestinationID");
            }

            if (!string.IsNullOrEmpty(adImpressionParameters.AdMediaType))
            {
                buf.PushString(adImpressionParameters.AdMediaType, "adMediaType");
            }

            if (adImpressionParameters.AdTimeWatchedMs is Int64 adTimeWatchedMsValue)
            {
                buf.PushInt64(adTimeWatchedMsValue, "adTimeWatchedMs");
            }

            if (adImpressionParameters.AdTimeCloseButtonShownMs is Int64 adTimeCloseButtonShownMsValue)
            {
                buf.PushInt64(adTimeCloseButtonShownMsValue, "adTimeCloseButtonShownMs");
            }

            if (adImpressionParameters.AdLengthMs is Int64 adLengthMsValue)
            {
                buf.PushInt64(adLengthMsValue, "adLengthMs");
            }

            if (adImpressionParameters.AdHasClicked is bool adHasClickedValue)
            {
                buf.PushBool(adHasClickedValue, "adHasClicked");
            }

            if (!string.IsNullOrEmpty(adImpressionParameters.AdSource))
            {
                buf.PushString(adImpressionParameters.AdSource, "adSource");
            }

            if (!string.IsNullOrEmpty(adImpressionParameters.AdStatusCallback))
            {
                buf.PushString(adImpressionParameters.AdStatusCallback, "adStatusCallback");
            }

            buf.PushEndEvent();
        }

        public void AcquisitionSource(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, AcquisitionSourceParameters acquisitionSourceParameters)
        {
            buf.PushStartEvent("acquisitionSource", datetime, 1, true);

            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);

            //other event parameters
            // Required
            buf.PushString(acquisitionSourceParameters.Channel, "acquisitionChannel");
            buf.PushString(acquisitionSourceParameters.CampaignId, "acquisitionCampaignId");
            buf.PushString(acquisitionSourceParameters.CreativeId, "acquisitionCreativeId");
            buf.PushString(acquisitionSourceParameters.CampaignName, "acquisitionCampaignName");
            buf.PushString(acquisitionSourceParameters.Provider, "acquisitionProvider");

            if (!string.IsNullOrEmpty(acquisitionSourceParameters.CampaignType))
            {
                buf.PushString(acquisitionSourceParameters.CampaignType, "acquisitionCampaignType");
            }

            if (!string.IsNullOrEmpty(acquisitionSourceParameters.Network))
            {
                buf.PushString(acquisitionSourceParameters.Network, "acquisitionNetwork");
            }

            if (!string.IsNullOrEmpty(acquisitionSourceParameters.CostCurrency))
            {
                buf.PushString(acquisitionSourceParameters.CostCurrency, "acquisitionCostCurrency");
            }

            if (acquisitionSourceParameters.Cost is float cost)
            {
                buf.PushFloat(cost, "acquisitionCost");
            }

            buf.PushEndEvent();
        }

        public void Transaction(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, TransactionParameters transactionParameters)
        {
            buf.PushStartEvent("transaction", datetime, 1, true);
            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);

            if (!string.IsNullOrEmpty(SdkVersion.SDK_VERSION))
            {
                buf.PushString(SdkVersion.SDK_VERSION, "sdkVersion");
            }

            if (!string.IsNullOrEmpty(transactionParameters.PaymentCountry))
            {
                buf.PushString(transactionParameters.PaymentCountry, "paymentCountry");
            }

            if (!string.IsNullOrEmpty(transactionParameters.ProductID))
            {
                buf.PushString(transactionParameters.ProductID, "productID");
            }

            if (transactionParameters.RevenueValidated.HasValue)
            {
                buf.PushInt64(transactionParameters.RevenueValidated.Value, "revenueValidated");
            }

            if (!string.IsNullOrEmpty(transactionParameters.TransactionID))
            {
                buf.PushString(transactionParameters.TransactionID, "transactionID");
            }

            if (!string.IsNullOrEmpty(transactionParameters.TransactionReceipt))
            {
                buf.PushString(transactionParameters.TransactionReceipt, "transactionReceipt");
            }

            if (!string.IsNullOrEmpty(transactionParameters.TransactionReceiptSignature))
            {
                buf.PushString(transactionParameters.TransactionReceiptSignature, "transactionReceiptSignature");
            }

            if (!string.IsNullOrEmpty(transactionParameters.TransactionServer?.ToString()))
            {
                buf.PushString(transactionParameters.TransactionServer.ToString(), "transactionServer");
            }

            if (!string.IsNullOrEmpty(transactionParameters.TransactorID))
            {
                buf.PushString(transactionParameters.TransactorID, "transactorID");
            }

            if (!string.IsNullOrEmpty(transactionParameters.StoreItemSkuID))
            {
                buf.PushString(transactionParameters.StoreItemSkuID, "storeItemSkuID");
            }

            if (!string.IsNullOrEmpty(transactionParameters.StoreItemID))
            {
                buf.PushString(transactionParameters.StoreItemID, "storeItemID");
            }

            if (!string.IsNullOrEmpty(transactionParameters.StoreID))
            {
                buf.PushString(transactionParameters.StoreID, "storeID");
            }

            if (!string.IsNullOrEmpty(transactionParameters.StoreSourceID))
            {
                buf.PushString(transactionParameters.StoreSourceID, "storeSourceID");
            }

            // Required
            buf.PushString(transactionParameters.TransactionName, "transactionName");
            buf.PushString(transactionParameters.TransactionType.ToString(), "transactionType");
            SetProduct(ref buf, "productsReceived", transactionParameters.ProductsReceived);
            SetProduct(ref buf, "productsSpent", transactionParameters.ProductsSpent);

            buf.PushEndEvent();
        }

        public void TransactionFailed(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier, TransactionFailedParameters parameters)
        {
            buf.PushStartEvent("transactionFailed", datetime, 1, true);
            // Event Params
            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);

            if (!string.IsNullOrEmpty(SdkVersion.SDK_VERSION))
            {
                buf.PushString(SdkVersion.SDK_VERSION, "sdkVersion");
            }

            if (!string.IsNullOrEmpty(parameters.PaymentCountry))
            {
                buf.PushString(parameters.PaymentCountry, "paymentCountry");
            }

            if (!string.IsNullOrEmpty(parameters.ProductID))
            {
                buf.PushString(parameters.ProductID, "productID");
            }

            if (parameters.RevenueValidated.HasValue)
            {
                buf.PushInt64(parameters.RevenueValidated.Value, "revenueValidated");
            }

            if (!string.IsNullOrEmpty(parameters.TransactionID))
            {
                buf.PushString(parameters.TransactionID, "transactionID");
            }

            if (!string.IsNullOrEmpty(parameters.TransactionServer?.ToString()))
            {
                buf.PushString(parameters.TransactionServer.ToString(), "transactionServer");
            }

            if (parameters.EngagementID != null)
            {
                buf.PushInt64((long)parameters.EngagementID, "engagementID");
            }

            if (!string.IsNullOrEmpty(parameters.GameStoreID))
            {
                buf.PushString(parameters.GameStoreID, "gameStoreID");
            }

            if (!string.IsNullOrEmpty(parameters.AmazonUserID))
            {
                buf.PushString(parameters.AmazonUserID, "amazonUserID");
            }

            if (parameters.IsInitiator != null)
            {
                buf.PushBool((bool)parameters.IsInitiator, "isInitiator");
            }

            if (!string.IsNullOrEmpty(parameters.StoreItemSkuID))
            {
                buf.PushString(parameters.StoreItemSkuID, "storeItemSkuID");
            }

            if (!string.IsNullOrEmpty(parameters.StoreItemID))
            {
                buf.PushString(parameters.StoreItemID, "storeItemID");
            }

            if (!string.IsNullOrEmpty(parameters.StoreID))
            {
                buf.PushString(parameters.StoreID, "storeID");
            }

            if (!string.IsNullOrEmpty(parameters.StoreSourceID))
            {
                buf.PushString(parameters.StoreSourceID, "storeSourceID");
            }

            // Required
            buf.PushString(parameters.TransactionName, "transactionName");
            buf.PushString(parameters.TransactionType.ToString(), "transactionType");
            SetProduct(ref buf, "productsReceived", parameters.ProductsReceived);
            SetProduct(ref buf, "productsSpent", parameters.ProductsSpent);

            buf.PushString(parameters.FailureReason, "failureReason");

            buf.PushEndEvent();
        }

        public void ClientDevice(ref Internal.IBuffer buf, DateTime datetime, StdCommonParams commonParams, string callingMethodIdentifier,
            string cpuType, string gpuType, Int64 cpuCores, Int64 ramTotal, Int64 screenWidth, Int64 screenHeight, Int64 screenDPI)
        {
            buf.PushStartEvent("clientDevice", datetime, 1, true);

            commonParams.SerializeCommonEventParams(ref buf, callingMethodIdentifier);

            // Schema: Optional
            buf.PushString(cpuType, "cpuType");
            buf.PushString(gpuType, "gpuType");
            buf.PushInt64(cpuCores, "cpuCores");
            buf.PushInt64(ramTotal, "ramTotal");
            buf.PushInt64(screenWidth, "screenWidth");
            buf.PushInt64(screenHeight, "screenHeight");
            buf.PushInt64(screenDPI, "screenResolution");

            buf.PushEndEvent();
        }

        void SetProduct(ref Internal.IBuffer buf, string productName, Product product)
        {
            buf.PushObjectStart(productName);

            if (product.RealCurrency.HasValue)
            {
                buf.PushObjectStart("realCurrency");
                buf.PushString(product.RealCurrency.Value.RealCurrencyType, "realCurrencyType");
                buf.PushInt64(product.RealCurrency.Value.RealCurrencyAmount, "realCurrencyAmount");
                buf.PushObjectEnd();
            }

            if (product.VirtualCurrencies != null && product.VirtualCurrencies.Count != 0)
            {
                buf.PushArrayStart("virtualCurrencies");
                foreach (var virtualCurrency in product.VirtualCurrencies)
                {
                    buf.PushObjectStart();
                    buf.PushObjectStart("virtualCurrency");
                    buf.PushString(virtualCurrency.VirtualCurrencyName, "virtualCurrencyName");
                    buf.PushString(virtualCurrency.VirtualCurrencyType.ToString(), "virtualCurrencyType");
                    buf.PushInt64(virtualCurrency.VirtualCurrencyAmount, "virtualCurrencyAmount");
                    buf.PushObjectEnd();
                    buf.PushObjectEnd();
                }
                buf.PushArrayEnd();
            }

            if (product.Items != null && product.Items.Count != 0)
            {
                buf.PushArrayStart("items");
                foreach (var item in product.Items)
                {
                    buf.PushObjectStart();
                    buf.PushObjectStart("item");
                    buf.PushString(item.ItemName, "itemName");
                    buf.PushString(item.ItemType, "itemType");
                    buf.PushInt64(item.ItemAmount, "itemAmount");
                    buf.PushObjectEnd();
                    buf.PushObjectEnd();
                }
                buf.PushArrayEnd();
            }

            buf.PushObjectEnd();
        }
    }
}
