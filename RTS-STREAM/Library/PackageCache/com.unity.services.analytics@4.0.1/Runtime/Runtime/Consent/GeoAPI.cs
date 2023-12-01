using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Networking;

[assembly: InternalsVisibleTo("Unity.Services.Analytics.Internal.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Unity.Services.Analytics.Internal
{
    interface IGeoAPI
    {
        Task<GeoIPResponse> MakeRequest();
    }

    class GeoAPI : IGeoAPI
    {
        readonly string m_PrivacyEndpoint = "https://pls.prd.mz.internal.unity3d.com/api/v1/user-lookup";

        public async Task<GeoIPResponse> MakeRequest()
        {
            var request = new UnityWebRequest(m_PrivacyEndpoint, UnityWebRequest.kHttpVerbGET)
            {
                timeout = 10,
                downloadHandler = new DownloadHandlerBuffer()
            };

            var async = request.SendWebRequest();

            while (!async.isDone)
            {
                await Task.Delay(1);
            }


#if UNITY_2020_1_OR_NEWER
            if (async.webRequest.result == UnityWebRequest.Result.ProtocolError ||
            async.webRequest.result == UnityWebRequest.Result.ConnectionError)
#else
            if (async.webRequest.isHttpError || async.webRequest.isNetworkError)
#endif
            {
                throw new ConsentCheckException(ConsentCheckExceptionReason.NoInternetConnection,
                    CommonErrorCodes.TransportError,
                    "The GeoIP request has failed - make sure you're connected to an internet connection and try again.",
                    null);
            }

            try
            {
                var response = JsonUtility.FromJson<GeoIPResponse>(async.webRequest.downloadHandler.text);
                if (response == null)
                {
                    throw new ConsentCheckException(ConsentCheckExceptionReason.Unknown, CommonErrorCodes.Unknown,
                        "The error occurred while performing the privacy GeoIP request. Please try again later.",
                        null);
                }

                return response;
            }
            catch (Exception)
            {
                throw new ConsentCheckException(ConsentCheckExceptionReason.DeserializationIssue, CommonErrorCodes.Unknown,
                    "The error occurred while deserializing the privacy GeoIP reseponse. Please try again later.",
                    null);
            }
        }
    }
}
