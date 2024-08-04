using System.Threading;
using Cysharp.Threading.Tasks;
using EidolonCorpTest.Interfaces;
using UnityEngine;
using UnityEngine.Networking;

namespace EidolonCorpTest.HttpSenders
{
    public class HttpSender : IHttpSender
    {
        public UniTask<(bool, long)> Post(string address, string data = null, CancellationToken token = default)
        {
            var request = UnityWebRequest.Put(address, data);
            request.method = UnityWebRequest.kHttpVerbPOST;
            SetHeaders(request);

            return Send(request, token);
        }

        protected async UniTask<(bool, long)> Send(UnityWebRequest request, CancellationToken token)
        {
            Debug.Log($"{request.method} {request.url}");
            await request.SendWebRequest().ToUniTask(cancellationToken: token);

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    return (true, request.responseCode);
                default:
                    return (false, request.responseCode);
            }
        }

        private void SetHeaders(UnityWebRequest request)
        {
            request.SetRequestHeader("content-type", "application/json");
        }
    }
}