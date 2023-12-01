using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Analytics.Internal;
using Unity.Services.Core;
using UnityEngine;

namespace Unity.Services.Analytics
{
    partial class AnalyticsServiceInstance
    {
        internal IConsentTracker ConsentTracker = new ConsentTracker();
        internal IAnalyticsForgetter analyticsForgetter;

        public async Task<List<string>> CheckForRequiredConsents()
        {
            var response = await ConsentTracker.CheckGeoIP();

            if (response.identifier == Consent.None)
            {
                return new List<string>();
            }

            if (ConsentTracker.IsConsentDenied())
            {
                return new List<string>();
            }

            if (!ConsentTracker.IsConsentGiven())
            {
                return new List<string> { response.identifier };
            }

            return new List<string>();
        }

        public void ProvideOptInConsent(string identifier, bool consent)
        {
            if (!ConsentTracker.IsGeoIpChecked())
            {
                throw new ConsentCheckException(ConsentCheckExceptionReason.ConsentFlowNotKnown,
                    CommonErrorCodes.Unknown,
                    "The required consent flow cannot be determined. Make sure CheckForRequiredConsents() method was successfully called.",
                    null);
            }

            if (consent == false)
            {
                if (ConsentTracker.IsConsentGiven(identifier))
                {
                    ConsentTracker.BeginOptOutProcess(identifier);
                    RevokeWithForgetEvent();
                    return;
                }

                Revoke();
            }

            ConsentTracker.SetUserConsentStatus(identifier, consent);
        }

        public void OptOut()
        {
            Debug.Log(ConsentTracker.IsConsentDenied()
                ? "This user has opted out. Any cached events have been discarded and no more will be collected."
                : "This user has opted out and is in the process of being forgotten...");

            if (ConsentTracker.IsConsentGiven())
            {
                // We have revoked consent but have not yet sent the ForgetMe signal
                // Thus we need to keep some of the dispatcher alive until that is done
                ConsentTracker.BeginOptOutProcess();
                RevokeWithForgetEvent();

                return;
            }

            if (ConsentTracker.IsOptingOutInProgress())
            {
                RevokeWithForgetEvent();
                return;
            }

            Revoke();
            ConsentTracker.SetDenyConsentToAll();
        }

        void Revoke()
        {
            // We have already been forgotten and so do not need to send the ForgetMe signal
            dataBuffer.ClearDiskCache();
            dataBuffer = new BufferRevoked();
            dataDispatcher = new Dispatcher(dataBuffer, new WebRequestHelper());
            ContainerObject.DestroyContainer();
        }

        internal void RevokeWithForgetEvent()
        {
            // Clear everything out of the real buffer and replace it with a dummy
            // that will swallow all events and do nothing
            dataBuffer.ClearBuffer();
            dataBuffer = new BufferRevoked();
            dataDispatcher = new Dispatcher(dataBuffer, new WebRequestHelper());

            analyticsForgetter = new AnalyticsForgetter(m_CollectURL,
                InstallId.GetOrCreateIdentifier(),
                Internal.Buffer.SaveDateTime(DateTime.Now),
                k_ForgetCallingId,
                ForgetMeEventUploaded, ConsentTracker);
            analyticsForgetter.AttemptToForget();
        }

        internal void ForgetMeEventUploaded()
        {
            ContainerObject.DestroyContainer();
            ConsentTracker.FinishOptOutProcess();

#if UNITY_ANALYTICS_EVENT_LOGS
            Debug.Log("User opted out successfully and has been forgotten!");
#endif
        }
    }
}
