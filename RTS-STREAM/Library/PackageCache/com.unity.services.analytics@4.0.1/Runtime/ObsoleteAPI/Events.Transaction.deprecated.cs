using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.Services.Analytics
{
    public static partial class Events
    {

        [Obsolete("This enum has been moved outside the Events class, the standalone enum should be used instead. This enum will be removed in an upcoming release.")]
        public enum TransactionServer
        {
            APPLE = 0,
            AMAZON = 1,
            GOOGLE = 2
        }

        [Obsolete("This enum has been moved outside the Events class, the standalone enum should be used instead. This enum will be removed in an upcoming release.")]
        public enum TransactionType
        {
            INVALID = 0,
            SALE = 1,
            PURCHASE = 2,
            TRADE = 3
        }

        [Obsolete("This struct has been moved outside the Events class, and it's parameters now conform to C# guidelines. Please use the standalone struct instead. This struct will be removed in an upcoming release.")]
        public struct Item
        {
            public string itemName;
            public string itemType;
            public Int64 itemAmount;
        }

        [Obsolete("This struct has been moved outside the Events class, and it's parameters now conform to C# guidelines. Please use the standalone struct instead. This struct will be removed in an upcoming release.")]
        public struct VirtualCurrency
        {
            public string virtualCurrencyName;
            public string virtualCurrencyType;
            public Int64 virtualCurrencyAmount;
        }

        [Obsolete("This struct has been moved outside the Events class, and it's parameters now conform to C# guidelines. Please use the standalone struct instead. This struct will be removed in an upcoming release.")]
        public struct RealCurrency
        {
            public string realCurrencyType;
            public Int64 realCurrencyAmount;
        }

        [Obsolete("This struct has been moved outside the Events class, and it's parameters now conform to C# guidelines. Please use the standalone struct instead. This struct will be removed in an upcoming release.")]
        public struct Product
        {
            //Optional
            public RealCurrency? realCurrency;
            public List<VirtualCurrency> virtualCurrencies;
            public List<Item> items;
        }

        [Obsolete("This struct has been moved outside the Events class, and it's parameters now conform to C# guidelines. Please use the standalone struct instead. This struct will be removed in an upcoming release.")]
        public struct TransactionParameters
        {
            [Obsolete]
            public bool? isInitiator;
            /// <summary>
            /// Optional.
            /// If this is left null or empty, the machine's locale will be used
            /// </summary>
            public string paymentCountry;
            public string productID;
            public Int64? revenueValidated;
            public string transactionID;
            public string transactionReceipt;
            public string transactionReceiptSignature;
            public TransactionServer? transactionServer;
            public string transactorID;
            public string storeItemSkuID;
            public string storeItemID;
            public string storeID;
            public string storeSourceID;
            //Required
            public string transactionName;
            public TransactionType transactionType;
            public Product productsReceived;
            public Product productsSpent;
        }

        /// <summary>
        /// Record a Transaction event.
        /// </summary>
        /// <param name="transactionParameters">(Required) Helper object to handle parameters.</param>
        [Obsolete("The interface provided by this method has moved to AnalyticsService.Instance.Transaction, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static void Transaction(TransactionParameters transactionParameters)
        {
            Analytics.TransactionServer? transactionServer = null;
            if (transactionParameters.transactionServer != null)
            {
                transactionServer = (Analytics.TransactionServer)(int)transactionParameters.transactionServer;
            }

            var newParameters = new Analytics.TransactionParameters
            {
                PaymentCountry = transactionParameters.paymentCountry,
                ProductID = transactionParameters.productID,
                RevenueValidated = transactionParameters.revenueValidated,
                TransactionID = transactionParameters.transactionID,
                TransactionReceipt = transactionParameters.transactionReceipt,
                TransactionReceiptSignature = transactionParameters.transactionReceiptSignature,
                TransactionServer = transactionServer,
                TransactorID = transactionParameters.transactorID,
                StoreItemSkuID = transactionParameters.storeItemSkuID,
                StoreItemID = transactionParameters.storeItemID,
                StoreID = transactionParameters.storeID,
                StoreSourceID = transactionParameters.storeSourceID,
                TransactionName = transactionParameters.transactionName,
                TransactionType = (Analytics.TransactionType)(int)transactionParameters.transactionType,
                ProductsReceived = ConvertProduct(transactionParameters.productsReceived),
                ProductsSpent = ConvertProduct(transactionParameters.productsSpent)
            };

            AnalyticsService.Instance.Transaction(newParameters);
        }

        static Analytics.Product ConvertProduct(Product from)
        {
            var newProduct = new Analytics.Product();

            if (from.items != null)
            {
                newProduct.Items = ConvertItems(from.items);
            }

            if (from.realCurrency != null)
            {
                var unwrappedRealCurrency = (RealCurrency)from.realCurrency;
                newProduct.RealCurrency = new Analytics.RealCurrency
                {
                    RealCurrencyAmount = unwrappedRealCurrency.realCurrencyAmount,
                    RealCurrencyType = unwrappedRealCurrency.realCurrencyType
                };
            }

            if (from.virtualCurrencies != null)
            {
                newProduct.VirtualCurrencies = from.virtualCurrencies.Select(vc =>
                {
                    var virtualCurrencyType = VirtualCurrencyType.GRIND;
                    if (!string.IsNullOrEmpty(vc.virtualCurrencyType) || !Enum.TryParse(vc.virtualCurrencyType, out virtualCurrencyType))
                    {
                        virtualCurrencyType = VirtualCurrencyType.GRIND;
                    }

                    return new Analytics.VirtualCurrency
                    {
                        VirtualCurrencyAmount = vc.virtualCurrencyAmount,
                        VirtualCurrencyName = vc.virtualCurrencyName,
                        VirtualCurrencyType = virtualCurrencyType
                    };
                }).ToList();
            }

            return newProduct;
        }

        static List<Analytics.Item> ConvertItems(List<Item> from)
        {
            return from.Select(i => new Analytics.Item { ItemAmount = i.itemAmount, ItemName = i.itemName, ItemType = i.itemType }).ToList();
        }
    }
}
