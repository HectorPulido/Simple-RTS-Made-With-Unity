using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Analytics.Internal
{
    interface IDispatcher
    {
        string CollectUrl { get; set; }
        Task Flush();
    }

    class Dispatcher : IDispatcher
    {
        readonly IBuffer m_DataBuffer;
        readonly IWebRequestHelper m_WebRequestHelper;

        internal readonly Dictionary<Guid, List<Buffer.Token>> Requests = new Dictionary<Guid, List<Buffer.Token>>();

        public string CollectUrl { get; set; }

        IConsentTracker ConsentTracker { get; }

        public Dispatcher(IBuffer buffer, IWebRequestHelper webRequestHelper, IConsentTracker consentTracker = null)
        {
            m_DataBuffer = buffer;
            m_WebRequestHelper = webRequestHelper;
            ConsentTracker = consentTracker;
        }

        public async Task Flush()
        {
            // Some sanity check that we aren't spinning out of control.
            // This should be very unlikely.
            if (Requests.Count > 128)
            {
                Debug.LogWarning("Analytics Dispatcher has reached limit of pending requests.");
                return;
            }

            // Also, check if the consent was definitely checked and given at this point.
            if (!ConsentTracker.IsGeoIpChecked() || !ConsentTracker.IsConsentGiven())
            {
                Debug.LogWarning("Required consent wasn't checked and given when trying to dispatch events, the events cannot be sent.");
                return;
            }

            await FlushBufferToService();
        }

        byte[] SerializeBuffer(List<Buffer.Token> tokens)
        {
            var collectData = m_DataBuffer.Serialize(tokens);
            if (string.IsNullOrEmpty(collectData))
            {
                return null;
            }

            return Encoding.UTF8.GetBytes(collectData);
        }

        async Task FlushBufferToService()
        {
            // Serialize it into a JSON Blob, then POST it to the Collect bulk URL.
            // 'Bulk Events' -> https://docs.deltadna.com/advanced-integration/rest-api/

            var tokens = m_DataBuffer.CloneTokens();
            var task = Task.Factory.StartNew(() => SerializeBuffer(tokens));
            var postBytes = await task;

            if (postBytes == null || postBytes.Length == 0)
            {
                return;
            }

            var request = m_WebRequestHelper.CreateWebRequest(CollectUrl, UnityWebRequest.kHttpVerbPOST, postBytes);

            if (ConsentTracker.IsGeoIpChecked() && ConsentTracker.IsConsentGiven())
            {
                foreach (var header in ConsentTracker.requiredHeaders)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            var requestId = Guid.NewGuid();
            // Callback
            // If the result is successful we will remove the request.
            // else if there was a failure, we insert the tokens back into the buffer.
            m_WebRequestHelper.SendWebRequest(request, delegate (long responseCode)
            {
#if UNITY_ANALYTICS_DEVELOPMENT
                Debug.LogFormat("AnalyticsRuntime: Web Callback - Request.Count = {0}", Requests.Count);
#endif

                if (!request.isNetworkError && responseCode == 204)
                {
#if UNITY_ANALYTICS_DEVELOPMENT
                    Debug.Assert(responseCode == 204, "AnalyticsRuntime: Incorrect response, check your JSON for errors.");
#endif

#if UNITY_ANALYTICS_EVENT_LOGS
                    Debug.LogFormat("Events uploaded successfully!");
#endif

                    m_DataBuffer.ClearDiskCache();
                }
                else
                {
                    // Reinsert the tokens back into the buffer.
                    m_DataBuffer.InsertTokens(Requests[requestId]);

#if UNITY_ANALYTICS_EVENT_LOGS
                    if (request.isNetworkError)
                    {
                        Debug.Log("Events failed to upload (network error) -- will retry at next heartbeat.");
                    }
                    else
                    {
                        Debug.LogFormat("Events failed to upload (code {0}) -- will retry at next heartbeat.", responseCode);
                    }
#endif

                    m_DataBuffer.FlushToDisk();
                }

                // Clear the request now that we are done.
                Requests.Remove(requestId);
                request.Dispose();
            });

#if UNITY_ANALYTICS_DEVELOPMENT
            Debug.Log("AnalyticsRuntime: Flush");
#endif

            Requests.Add(requestId, tokens);

#if UNITY_ANALYTICS_DEVELOPMENT
            Debug.LogFormat("AnalyticsRuntime: Request In Queue - Request.Count = {0}", Requests.Count);
#endif

#if UNITY_ANALYTICS_EVENT_LOGS
            Debug.Log("Uploading events...");
#endif
        }
    }
}
