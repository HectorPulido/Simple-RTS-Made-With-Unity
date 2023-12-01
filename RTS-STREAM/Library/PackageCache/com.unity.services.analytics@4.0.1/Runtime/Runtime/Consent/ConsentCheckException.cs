using System;
using Unity.Services.Core;
using UnityEngine.Scripting;

namespace Unity.Services.Analytics
{
    /// <summary>
    /// Represents geolocation consent errors.  
    /// </summary>
    [Preserve]
    public enum ConsentCheckExceptionReason
    {
        Unknown = 0,
        DeserializationIssue = 1,
        NoInternetConnection = 2,
        InvalidConsentFlow = 3,
        ConsentFlowNotKnown = 4,
    }

    public class ConsentCheckException : RequestFailedException
    {
        [Preserve] public ConsentCheckExceptionReason Reason { get; private set; }

        public ConsentCheckException(ConsentCheckExceptionReason reason, int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        {
            Reason = reason;
        }
    }
}
