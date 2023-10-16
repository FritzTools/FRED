using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace FRED.Core.Fritzbox.Networking.HTTP {
    public class FormDataContent : StringContent {
        public FormDataContent(String name, String content) : base(content) {
            this.Headers.Clear();
            this.Headers.TryAddWithoutValidation("Content-Disposition", "form-data; name=\"" + name + "\"");
        }
    }
}
