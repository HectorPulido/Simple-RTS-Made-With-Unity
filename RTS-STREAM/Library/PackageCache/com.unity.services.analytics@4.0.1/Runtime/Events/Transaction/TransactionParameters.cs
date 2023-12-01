using System;

namespace Unity.Services.Analytics
{
    public struct TransactionParameters
    {
        /// <summary>
        /// Optional.
        /// If this is left null or empty, the machine's locale will be used
        /// </summary>
        public string PaymentCountry;
        public string ProductID;
        public Int64? RevenueValidated;
        public string TransactionID;
        public string TransactionReceipt;
        public string TransactionReceiptSignature;
        public TransactionServer? TransactionServer;
        public string TransactorID;
        public string StoreItemSkuID;
        public string StoreItemID;
        public string StoreID;
        public string StoreSourceID;
        //Required
        public string TransactionName;
        public TransactionType TransactionType;
        public Product ProductsReceived;
        public Product ProductsSpent;
    }
}
