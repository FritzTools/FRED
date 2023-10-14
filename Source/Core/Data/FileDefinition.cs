using Newtonsoft.Json;
using System;

namespace FRED.Core.Data {
    public class FileDefinition {
        [JsonProperty]
        public String? File { get; set; } = null;

        [JsonProperty]
        public String? Description { get; set; } = null;
    }
}
