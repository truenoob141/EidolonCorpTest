using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Assertions;

namespace EidolonCorpTest.Data
{
    public class AnalyticCollection
    {
        [JsonProperty("events")]
        public List<AnalyticEvent> Events { get; }
        
        public AnalyticCollection(ICollection<AnalyticEvent> events)
        {
            Assert.IsNotNull(events);
            
            Events = new List<AnalyticEvent>(events);
        }
    }
}