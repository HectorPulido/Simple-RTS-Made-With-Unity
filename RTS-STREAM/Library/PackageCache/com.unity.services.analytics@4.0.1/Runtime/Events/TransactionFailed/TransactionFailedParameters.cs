using System;

namespace Unity.Services.Analytics
{
    public struct TransactionFailedParameters
    {
        /// <summary>
        /// Optional.
        /// If this is left null or empty, the machine's locale will be used
        /// </summary>
        public string PaymentCountry;
        public long? EngagementID;
        public bool? IsInitiator;
        public string StoreID;
        public string StoreSourceID;
        public string TransactionID;
        public string StoreItemID;
        public string AmazonUserID;
        public string StoreItemSkuID;
        public string ProductID;
        public string GameStoreID;
        public TransactionServer? TransactionServer;
        public long? RevenueValidated;

        //Required
        public string TransactionName;
        public TransactionType TransactionType;
        public Product ProductsReceived;
        public Product ProductsSpent;
        public string FailureReason;
    }
}
