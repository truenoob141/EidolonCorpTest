// Примечание:
// Я не стал усложнять и делать сохранение локальных данных
// для больших объемов данных (оптимизацию), т.к. это усложило бы код
// и я не уверен что это требовалось (в задании не сказано).
// В случае если это необходимо, то я сделал бы на потоках со сбросом данных на диск.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using EidolonCorpTest.Data;
using EidolonCorpTest.HttpSenders;
using EidolonCorpTest.Interfaces;
using Newtonsoft.Json;
using UnityEngine;

namespace EidolonCorpTest
{
    public class AnalyticService : MonoBehaviour
    {
        [SerializeField]
        private AnalyticSettings _analyticSettings;

        // Mockup sender
        private readonly IHttpSender _httpSender = new HttpSenderMockup();
        
        // Queue of events
        private readonly List<AnalyticEvent> _events = new();

        // Is sending now?
        private bool _isSending;
        
        // Time when we start tracking events
        private float _startTrackTime;

        private string _localStoragePath;
        private CancellationToken _token;

        private void Awake()
        {
            _localStoragePath = Path.Combine(Application.persistentDataPath, _analyticSettings.localFileName);
            _token = this.GetCancellationTokenOnDestroy();
            
            // Load events from local storage
            ReadFromLocalStorage();
        }

        private void Update()
        {
            if (_isSending)
                return;

            float timeLeft = GetAnalyticSendTimeLeft();
            if (timeLeft > 0)
                return;

            SendAllEvents(_token).Forget();
        }

        public void TrackEvent(string type, string data)
        {
            if (_events.Count == 0)
                _startTrackTime = Time.time;

            Debug.Log($"Track event {type}, data: {data}");
            _events.Add(new AnalyticEvent(type, data));

            SaveToLocalStorage();
        }

        private async UniTask SendAllEvents(CancellationToken token)
        {
            _isSending = true;

            try
            {
                int count = _events.Count;

                var collection = new AnalyticCollection(_events);
                (bool success, long code) = await _httpSender.Post(
                    _analyticSettings.serverUrl,
                    JsonConvert.SerializeObject(collection),
                    token);

                Debug.Log($"Send {count} events. Success: {success}, Code: {code}");
                if (!success || code != 200)
                    return;

                _events.RemoveRange(0, count);

                ClearLocalStorage();
            }
            finally
            {
                _startTrackTime = Time.time;
                _isSending = false;
            }
        }

        private float GetAnalyticSendTimeLeft()
        {
            if (_events.Count == 0)
                return Mathf.Infinity;

            return _analyticSettings.cooldownBeforeSend - (Time.time - _startTrackTime);
        }

        private void ReadFromLocalStorage()
        {
            CreateLocalStorageIfNotExists();

            try
            {
                string json = File.ReadAllText(_localStoragePath, Encoding.UTF8);
                if (string.IsNullOrEmpty(json))
                    return;

                var analyticEvents = JsonConvert.DeserializeObject<AnalyticEvent[]>(json);
                if (analyticEvents == null)
                    return;

                Debug.Log("Add from local storage " + analyticEvents.Length);
                if (_events.Count == 0 && analyticEvents.Length > 0)
                    _startTrackTime = Time.time - _analyticSettings.cooldownBeforeSend;
                
                _events.AddRange(analyticEvents);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to read from local storage");
                Debug.LogException(ex);
            }
        }

        private void SaveToLocalStorage()
        {
            CreateLocalStorageIfNotExists();

            try
            {
                string json = JsonConvert.SerializeObject(_events);
                File.WriteAllText(_localStoragePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to save to local storage");
                Debug.LogException(ex);
            }
        }

        private void ClearLocalStorage()
        {
            try
            {
                if (File.Exists(_localStoragePath))
                    File.WriteAllText(_localStoragePath, "[]");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to clear local storage");
                Debug.LogException(ex);
            }
        }

        private void CreateLocalStorageIfNotExists()
        {
            if (File.Exists(_localStoragePath))
                return;

            string directory = Path.GetDirectoryName(_localStoragePath);
            if (directory == null)
                throw new InvalidOperationException("Failed to create local storage: No path");

            try
            {
                Directory.CreateDirectory(directory);
                using (var stream = File.Create(_localStoragePath))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes("[]");
                    stream.Write(bytes);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to create local storage");
                Debug.LogException(ex);
            }
        }
    }
}