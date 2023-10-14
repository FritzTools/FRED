using FRED.Core.Parser;
using System;

namespace FRED.Core.Events {
    public class PropertiesEvent : EventArgs {
        public PropertyList? Properties { get; set; }

        public PropertiesEvent(PropertyList? list) {
            Properties = list;
        }
    }
}
