
using Newtonsoft.Json;

namespace EidolonCorpTest.Data
{
    public class AnalyticEvent
    {
        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("data")]
        public string Data { get; }
        
        public AnalyticEvent(string type, string data)
        {
            Type = type;
            Data = data;
        }
    }
}