using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EidolonCorpTest
{
    [RequireComponent(typeof(AnalyticService))]
    public class Tester : MonoBehaviour
    {
        [SerializeField]
        private float _interval = 5f;

        private AnalyticService _service;

        private float _lastIterationTime;

        private Func<CancellationToken, UniTask>[] _testCases;

        private void Awake()
        {
            _testCases = new Func<CancellationToken, UniTask>[]
                { Case01, Case02 };

            _service = GetComponent<AnalyticService>();
            _lastIterationTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - _lastIterationTime < _interval)
                return;

            _lastIterationTime = Time.time;
            
            float random = Random.value;
            int index = (int) (random / (1f / _testCases.Length));

            var token = this.GetCancellationTokenOnDestroy();
            _testCases[index](token).Forget();
        }

        private UniTask Case01(CancellationToken token)
        {
            _service.TrackEvent("Case01", "Test data");
            return UniTask.CompletedTask;
        }

        private UniTask Case02(CancellationToken token)
        {
            int count = Random.Range(6, 20);
            for (int i = 0; i < count; ++i)
                _service.TrackEvent("Case02", "Test " + i);

            return UniTask.CompletedTask;
        }
    }
}