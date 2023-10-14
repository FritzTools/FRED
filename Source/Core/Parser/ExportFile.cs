using FRED.Core.Data;
using FRED.Core.Events;
using FRED.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FRED.Core.Parser {
    public class ExportFile : Consumable {
        private List<Action> on_ready                            = new List<Action>();
        private Dictionary<String, FileContainer?> files         = new Dictionary<String, FileContainer?>();
        private Dictionary<String, FileDefinition?> definitions  = new Dictionary<String, FileDefinition?>();
        private String? file             = null;
        private String? content          = null;
        private String? name             = null;
        private String? checksum         = null;
        private Boolean is_ready         = false;
        private PropertyList? properties;
        public event EventHandler? observer;

        public ExportFile(String? file) {
            this.file = file;

            this.LoadDefinitions();
        }

        private void LoadDefinitions() {
           String content               = File.ReadAllText("./Definitions/Files.json", Encoding.UTF8);
            List<FileDefinition?>? json = JsonConvert.DeserializeObject<List<FileDefinition?>?>(content);
            
            if(json != null) {
                json.ForEach(file => {
                    if(file  != null && file.File != null) { 
                        definitions.Add(file.File, file);
                    }
                });
            }
        }

        public FileDefinition? GetDefinition(String? name) {
            if(this.definitions == null || name == null) {
                return null;
            }

            return this.definitions.GetValueOrDefault(name, null);
        }

        public void SetObserver(Changeable? instance) {
            if(instance == null) {
                return;
            }

            observer += instance.OnChange;
        }

        public Boolean IsReady() {
            return this.is_ready;
        }

        public void OnReady(Action callback) {
            this.on_ready.Add(callback);
        }

        public bool Get(String name, Action<object?> callback) {
            if(!this.is_ready) {
                return false;
            }

            System.Diagnostics.Debug.Print("[REQUEST] " + name);

            switch (name) {
                case "Info":
                    DateTime time_changed = new DateTime();
                    DateTime time_created = new DateTime();

                    if(this.file != null) {
                        time_changed = File.GetLastWriteTime(this.file);
                        time_created = File.GetCreationTime(this.file);
                    }

                    callback.Invoke((object) new InfoData () {
                        Name        = this.name,
                        File        = this.file,
                        Checksum    = this.checksum,
                        TimeChanged = time_changed,
                        TimeCreated = time_created,
                        Files       = this.files.Count
                    });
                break;
                case "Name":
                    callback.Invoke(this.name);
                break;
                case "File":
                    callback.Invoke(this.file);
                break;
                case "Checksum":
                    callback.Invoke(this.checksum);
                break;
                case "Properties":
                    callback.Invoke( this.properties);
                break;
                default:
                    if(files.ContainsKey(name)) {
                        callback.Invoke(files[name]);
                    } else {
                        callback.Invoke(null);
                    }
                break;
            }

            return true;
        }

        public PropertyList? GetProperties() {
            return this.properties;
        }

        public void Parse() {
            if(this.file == null) {
                System.Diagnostics.Debug.Print("File is empty?");
                return;
            }

            this.content = File.ReadAllText(this.file, Encoding.UTF8);

            Regex export    = new Regex(@"^\*{4}\s(?<name>[A-Za-z!\s0-9]+)?\sCONFIGURATION\sEXPORT\s(?<content>[\s\S]*?)?\*{4}\sEND\sOF\sEXPORT\s(?<checksum>[A-F0-9]+)?\s\*{4}\s*?$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match match     = export.Match(this.content);

            if(!match.Success) {
                System.Diagnostics.Debug.Print("No Matches Found, bad export file?");
                return;
            }
           
            this.name       = match.Groups["name"].Value;
            this.checksum   = match.Groups["checksum"].Value;
            this.content    = match.Groups["content"].Value;

            // Get Properties
            Regex properties    = new Regex(@"^\A(?<properties>[\s\S]*?)\s(\*{4})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            match               = properties.Match(this.content);
            String props        = match.Groups["properties"].Value;
            this.content        = this.content.Replace(props, "");
            this.properties     = new PropertyList(props);

            observer?.Invoke(this, new PropertiesEvent(this.properties));

            this.ParseFiles();

            this.is_ready = true;

            foreach(Action callback in this.on_ready) {
                callback();
            }
        }

        public void ParseFiles() {
            Regex files = new Regex(@"^\*{4}\s(?:CRYPTED)?(?<type>(CFG|BIN|B64))?FILE:(?<name>.*)?\s(?<content>[\s\S]*?)?\s\*{4}\sEND\sOF\sFILE\s\*{4}\s*?$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if(this.content != null) {
                foreach (Match match in files.Matches(this.content)) {
                    FileContainer file = new FileContainer(match.Groups["name"].Value, match.Groups["type"].Value);

                    if(file != null) {
                        String? name                = file.GetName();
                        FileDefinition? definition  = this.GetDefinition(name);

                        if(!match.Groups["crypted"].Value.Equals("")) {
                            file.SetCrypted();
                        }

                        if(definition != null) {
                            file.SetDefinition(ref definition);
                        }

                        file.SetContent(match.Groups["content"].Value);

                        // @ToDo Parse Content

                        if(name != null) { 
                            this.files.Add(name, file);
                            observer?.Invoke(this, new FileEvent(file));
                        }
                    }
                }
            }
        }
    }
}
