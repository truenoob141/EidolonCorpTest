using UnityEngine;

namespace EidolonCorpTest
{
    [CreateAssetMenu(menuName = "AnalyticSettings")]
    public class AnalyticSettings : ScriptableObject
    {
        public string serverUrl = "http://example.com/";
        public string localFileName = "analytic.json";
        
        /// <summary>
        /// In seconds
        /// </summary>
        public float cooldownBeforeSend = 1f;
    }
}