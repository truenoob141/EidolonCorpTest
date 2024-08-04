using System.Threading;
using Cysharp.Threading.Tasks;
using EidolonCorpTest.Interfaces;
using UnityEngine;

namespace EidolonCorpTest.HttpSenders
{
    public class HttpSenderMockup : IHttpSender
    {
        public async UniTask<(bool, long)> Post(string address, string data = null, CancellationToken token = default)
        {
            await UniTask.Delay(Random.Range(0, 1000), cancellationToken: token);

            float random = Random.value;
            if (random > 0.5f)
                return (true, 200);

            if (random > 0.4f)
                return (true, 404);

            if (random > 0.3f)
                return (false, 200);

            return (false, 501);
        }
    }
}