using System;

namespace FRED.Core.Data {
    public class ConsumingCallback {
        public String? Name { get; set; } = null;
        public Action<object?>? Callback { get; set; } = null;
    }
}
