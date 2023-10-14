using FRED.Core.Data;
using System;

namespace FRED.Core.Events {
    public class FileEvent : EventArgs {
        public FileContainer? File { get; set; }

        public FileEvent(FileContainer? container) {
            File = container;
        }
    }
}
