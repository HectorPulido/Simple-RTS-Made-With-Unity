namespace Unity.Services.Analytics
{
    public class AcquisitionSourceParameters
    {
        // Required
        public string Channel;
        public string CampaignId;
        public string CreativeId;
        public string CampaignName;
        public string Provider;
        // Optional
        public float? Cost;
        public string CostCurrency;
        public string Network;
        public string CampaignType;
    }
}
