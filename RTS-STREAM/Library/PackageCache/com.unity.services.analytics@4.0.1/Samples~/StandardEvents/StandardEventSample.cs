using System.Collections.Generic;

namespace Unity.Services.Analytics
{
    public class StandardEventSample
    {
        public static void RecordMinimalAdImpressionEvent()
        {
            var args = new AdImpressionParameters
            {
                AdCompletionStatus = AdCompletionStatus.Completed,
                AdProvider = AdProvider.UnityAds,
                PlacementName = "PLACEMENTNAME",
                PlacementID = "PLACEMENTID"
            };
            AnalyticsService.Instance.AdImpression(args);
        }

        public static void RecordCompleteAdImpressionEvent()
        {
            var args = new AdImpressionParameters
            {
                AdCompletionStatus = AdCompletionStatus.Completed,
                AdProvider = AdProvider.UnityAds,
                PlacementName = "PLACEMENTNAME",
                PlacementID = "PLACEMENTID",
                PlacementType = AdPlacementType.BANNER,
                AdEcpmUsd = 123.4,
                SdkVersion = "123.4",
                AdImpressionID = "IMPRESSIVE",
                AdStoreDstID = "DSTID",
                AdMediaType = "MOVIE",
                AdTimeWatchedMs = 1234,
                AdTimeCloseButtonShownMs = 5678,
                AdLengthMs = 2345,
                AdHasClicked = false,
                AdSource = "ADSRC",
                AdStatusCallback = "STATCALL"
            };

            AnalyticsService.Instance.AdImpression(args);
        }

        public static void RecordSaleTransactionWithOnlyRequiredValues()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product(),
                ProductsSpent = new Product(),
                TransactionName = "emptySale",
                TransactionType = TransactionType.SALE
            });
        }

        public static void RecordSaleTransactionWithRealCurrency()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product
                {
                    RealCurrency = new RealCurrency
                    {
                        RealCurrencyType = "EUR",
                        RealCurrencyAmount = AnalyticsService.Instance.ConvertCurrencyToMinorUnits("EUR", 3.99)
                    }
                },
                ProductsSpent = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "thePickOfDestiny",
                            ItemAmount = 1,
                            ItemType = "collectable"
                        }
                    }
                },
                TransactionName = "sellItem",
                TransactionType = TransactionType.SALE
            });
        }

        public static void RecordSaleTransactionWithVirtualCurrency()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product
                {
                    VirtualCurrencies = new List<VirtualCurrency>
                    {
                        new VirtualCurrency
                        {
                            VirtualCurrencyType = VirtualCurrencyType.GRIND,
                            VirtualCurrencyAmount = 125000,
                            VirtualCurrencyName = "Cor"
                        }
                    }
                },
                ProductsSpent = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "elucidator",
                            ItemAmount = 1,
                            ItemType = "sword"
                        }
                    }
                },
                TransactionName = "sellItem",
                TransactionType = TransactionType.SALE
            });
        }

        public static void RecordSaleTransactionWithMultipleVirtualCurrencies()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product
                {
                    VirtualCurrencies = new List<VirtualCurrency>
                    {
                        new VirtualCurrency
                        {
                            VirtualCurrencyType = VirtualCurrencyType.PREMIUM,
                            VirtualCurrencyAmount = 100,
                            VirtualCurrencyName = "Soul Points"
                        },
                        new VirtualCurrency
                        {
                            VirtualCurrencyType = VirtualCurrencyType.GRIND,
                            VirtualCurrencyAmount = 50000,
                            VirtualCurrencyName = "Gold Coins"
                        },
                    }
                },
                ProductsSpent = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "darkRepulser",
                            ItemAmount = 1,
                            ItemType = "weapon"
                        }
                    }
                },
                TransactionName = "sellItem",
                TransactionType = TransactionType.SALE
            });
        }

        public static void RecordSaleEventWithOneItem()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "cabbage",
                            ItemAmount = 50,
                            ItemType = "food"
                        }
                    }
                },
                ProductsSpent = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "marketStall",
                            ItemAmount = 1,
                            ItemType = "special"
                        }
                    }
                },
                TransactionName = "tradeItems",
                TransactionType = TransactionType.SALE
            });
        }

        public static void RecordSaleEventWithMultipleItems()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "pancake",
                            ItemAmount = 2,
                            ItemType = "food",
                        },
                        new Item
                        {
                            ItemName = "whippedCream",
                            ItemAmount = 165,
                            ItemType = "food",
                        }
                    }
                },
                ProductsSpent = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "flour",
                            ItemAmount = 100,
                            ItemType = "food",
                        },
                        new Item
                        {
                            ItemName = "egg",
                            ItemAmount = 1,
                            ItemType = "food",
                        },
                        new Item
                        {
                            ItemName = "milk",
                            ItemAmount = 200,
                            ItemType = "food",
                        },
                        new Item
                        {
                            ItemName = "salt",
                            ItemAmount = 1,
                            ItemType = "food",
                        },
                        new Item
                        {
                            ItemName = "heavyCream",
                            ItemAmount = 150,
                            ItemType = "food",
                        },
                        new Item
                        {
                            ItemName = "sugar",
                            ItemAmount = 15,
                            ItemType = "food",
                        }
                    }
                },
                TransactionName = "tradeItems",
                TransactionType = TransactionType.SALE
            });
        }

        public static void RecordSaleEventWithOptionalParameters()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                PaymentCountry = "PL",
                ProductID = "productid987",
                RevenueValidated = 999,
                TransactionID = "0118-999-881-999-119-725-3",
                TransactionReceipt = "transactionrecepit",
                TransactionReceiptSignature = "signature",
                TransactionServer = TransactionServer.APPLE,
                TransactorID = "transactorid-0118-999-881-999-119-725-3",
                StoreItemSkuID = "storeitemskuid",
                StoreItemID = "storeitemid",
                StoreID = "storeid",
                StoreSourceID = "storesourceid",
                ProductsReceived = new Product(),
                ProductsSpent = new Product(),
                TransactionName = "transactionName",
                TransactionType = TransactionType.SALE
            });
        }

        public static void RecordAcquisitionSourceEventWithOnlyRequiredValues()
        {
            AnalyticsService.Instance.AcquisitionSource(new AcquisitionSourceParameters
            {
                Channel = "CHNL",
                CampaignId = "123-456-efg",
                CreativeId = "cre-ati-vei-d",
                CampaignName = "Interstitial:Halloween21",
                Provider = "AppsFlyer"
            });
        }

        public static void RecordAcquisitionSourceEventWithOptionalParameters()
        {
            AnalyticsService.Instance.AcquisitionSource(new AcquisitionSourceParameters
            {
                Channel = "CHNL",
                CampaignId = "123-456-efg",
                CreativeId = "cre-ati-vei-d",
                CampaignName = "Interstitial:Halloween21",
                Provider = "AppsFlyer",
                CampaignType = "CPI",
                Cost = 123.4F,
                CostCurrency = "BGN",
                Network = "Ironsource",
            });
        }

        public static void RecordPurchaseEventWithOneItem()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "nerveGear",
                            ItemAmount = 1,
                            ItemType = "electronics",
                        }
                    }
                },
                ProductsSpent = new Product
                {
                    RealCurrency = new RealCurrency
                    {
                        RealCurrencyAmount = AnalyticsService.Instance.ConvertCurrencyToMinorUnits("JPY", 39800),
                        RealCurrencyType = "JPY"
                    }
                },
                TransactionName = "itemPurchase",
                TransactionType = TransactionType.PURCHASE
            });
        }

        public static void RecordPurchaseEventWithMultipleItems()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "magicarp",
                            ItemAmount = 1,
                            ItemType = "pokemon",
                        },
                        new Item
                        {
                            ItemName = "rareCandy",
                            ItemAmount = 20,
                            ItemType = "item",
                        }
                    }
                },
                ProductsSpent = new Product
                {
                    VirtualCurrencies = new List<VirtualCurrency>
                    {
                        new VirtualCurrency
                        {
                            VirtualCurrencyType = VirtualCurrencyType.GRIND,
                            VirtualCurrencyAmount = 200500,
                            VirtualCurrencyName = "Pokemon Dollar"
                        },
                    }
                },
                TransactionName = "itemPurchase",
                TransactionType = TransactionType.PURCHASE
            });
        }

        public static void RecordPurchaseEventWithMultipleCurrencies()
        {
            AnalyticsService.Instance.Transaction(new TransactionParameters
            {
                ProductsReceived = new Product
                {
                    Items = new List<Item>
                    {
                        new Item
                        {
                            ItemName = "holySwordExcalibur",
                            ItemAmount = 1,
                            ItemType = "weapon"
                        }
                    }
                },
                ProductsSpent = new Product
                {
                    VirtualCurrencies = new List<VirtualCurrency>
                    {
                        new VirtualCurrency
                        {
                            VirtualCurrencyType = VirtualCurrencyType.GRIND,
                            VirtualCurrencyAmount = 4000000,
                            VirtualCurrencyName = "Cor"
                        },
                        new VirtualCurrency
                        {
                            VirtualCurrencyType = VirtualCurrencyType.PREMIUM,
                            VirtualCurrencyAmount = 50000,
                            VirtualCurrencyName = "Credit"
                        }
                    }
                },
                TransactionName = "itemPurchase",
                TransactionType = TransactionType.PURCHASE
            });
        }
    }
}
