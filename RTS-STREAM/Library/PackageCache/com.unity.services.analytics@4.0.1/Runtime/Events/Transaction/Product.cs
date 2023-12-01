using System.Collections.Generic;

namespace Unity.Services.Analytics
{
    public struct Product
    {
        //Optional
        public RealCurrency? RealCurrency;
        public List<VirtualCurrency> VirtualCurrencies;
        public List<Item> Items;
    }
}
