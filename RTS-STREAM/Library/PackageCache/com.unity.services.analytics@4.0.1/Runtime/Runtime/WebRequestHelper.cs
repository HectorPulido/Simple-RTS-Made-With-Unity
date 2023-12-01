using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Analytics.Internal
{
    interface IWebRequest
    {
        UnityWebRequestAsyncOperation SendWebRequest();
        UploadHandler uploadHandler { get; set; }
        void SetRequestHeader(string key, string value);
        bool isNetworkError { get; }
        void Dispose();
    }

    interface IWebRequestHelper
    {
        IWebRequest CreateWebRequest(string url, string method, byte[] postBytes);
        void SendWebRequest(IWebRequest request, Action<long> onCompleted);
    }

    class AnalyticsWebRequest : UnityWebRequest, IWebRequest
    {
        internal AnalyticsWebRequest(string url, string method) : base(url, method) { }
    }

    class WebRequestHelper : IWebRequestHelper
    {
        public IWebRequest CreateWebRequest(string url, string method, byte[] postBytes)
        {
            var request = new AnalyticsWebRequest(url, method);
            var upload = new UploadHandlerRaw(postBytes)
            {
                contentType = "application/json"
            };
            request.uploadHandler = upload;
            return request;
        }

        public void SendWebRequest(IWebRequest request, Action<long> onCompleted)
        {
            var requestOp = request.SendWebRequest();
            requestOp.completed += delegate
            {
                onCompleted(requestOp.webRequest.responseCode);
            };
        }
    }
}
