using System;

namespace FRED.Core.Data {
    public class InfoData {
        public String? Name { get; set; } = null;
        public String? File { get; set; } = null;
        public String? Checksum { get; set; } = null;
        public int? Files { get; set; } = null;
        public DateTime? TimeCreated { get; set; } = null;
        public DateTime? TimeChanged { get; set; } = null;
    }
}
