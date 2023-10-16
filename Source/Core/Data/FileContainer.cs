using System;
using System.Collections.Generic;
using System.Linq;
using FRED.Core.Parser;

namespace FRED.Core.Data {
    public class FileContainer {
        private String? name                = null;
        private String? type                = null;
        private String? subtype             = null;
        private String? content             = null;
        private bool crypted                = false;
        private FileDefinition? definition  = null;
        private IToken? groups              = null;

        public FileContainer(String name, String type) {
            this.name = name;
            this.type = type;
        }

        public void SetDefinition(ref FileDefinition definition) {
            this.definition = definition;
        }

        public new String? GetType() {
            return this.type;
        }

        public String GetSubType() {
            if(this.subtype == null) {
                return "";
            }

            return this.subtype;
        }

        public bool HasSubType() {
            return this.subtype != null;
        }

        public void SetContent(String content) {
            this.content = content;
            this.UpdateSubType();
        }

        public void UpdateSubType() {

            if(this.content == null) {
                return;
            }

            String data = this.content;

            switch (this.type) {
                case "B64":
                    data            = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
                    this.subtype    = "Properties";
                break;
            }

            if(!string.IsNullOrEmpty(data) && data.TrimStart().StartsWith("<")) {
                this.subtype = "XML";
            } else if(data.Any(ch => char.IsControl(ch) && ch != '\r' && ch != '\n')) {
                this.subtype = "Binary";
            } else if(data.Trim().Equals("")) {
                this.subtype = "Unknown";
            }
        }

        public void SetCrypted() {
            this.crypted = true;
        }

        public void SetUnencrypted() {
            this.crypted = false;
        }

        public bool IsCrypted() {
            return this.crypted;
        }

        public String? GetDescription() {
            return this.definition?.Description;
        }

        public String? GetName() { 
            return this.name; 
        }

        public String? GetSource() {
            if(this.content == null) {
                return null;
            }

            switch (this.type) {
                case "CFG":
                    return this.content;
                case "B64":
                    return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(this.content));
                case "BIN":
                    return this.content;
            }

            return "<Unknown " + this.type  + ">";
        }
        public void SetGroups(IToken groups) {
            this.groups = groups;
        }

        public IToken? GetGroups() {
            return this.groups;
        }
    }
}
