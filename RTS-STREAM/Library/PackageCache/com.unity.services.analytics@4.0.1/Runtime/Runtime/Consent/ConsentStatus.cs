namespace Unity.Services.Analytics.Internal
{
    enum ConsentStatus
    {
        Unknown = 0, //required consents were not checked with GeoIP lookup
        Forgetting = 1, //in the process of opting out
        OptedOut = 2, //opted out, ForgetMe event sent to the backend
        NotRequired = 3, //explicit user consent is not required
        RequiredButUnchecked = 4, //explicit user consent is required
        ConsentGiven = 5, //user gave the consent
        ConsentDenied = 6 //user denied the consent - no consent was given at any point,
                          //no ForgetMe event was sent to the backend
    }
}
