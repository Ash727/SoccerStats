using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerStats
{
    class SentimentResponse
    {
        [JsonProperty(PropertyName = "documents")]
        public List<Sentiement> sentiemnts { get; set; }
        public object[] errors { get; set; }
        
    }

        public class Sentiement
        {
            [JsonProperty(PropertyName ="score")]
            public float score { get; set; }
            [JsonProperty(PropertyName = "id")]
            public string id { get; set; }
        }

}
