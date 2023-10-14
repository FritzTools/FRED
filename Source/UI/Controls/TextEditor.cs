using FRED.Core.Interfaces;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace FRED.UI.Controls {
    public class TextEditor : UserControl, EditorInstance {
        protected static String TAG_ROOT            = "FAKEROOTFORPARSER";
        ICSharpCode.AvalonEdit.TextEditor editor    = new ICSharpCode.AvalonEdit.TextEditor() {
            SyntaxHighlighting            = HighlightingManager.Instance.GetDefinition("ini"),
            ShowLineNumbers               = true,
            VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            FontFamily                    = new System.Windows.Media.FontFamily("Consolas"),
            FontSize                      = 16,
            Options                       = {
                InheritWordWrapIndentation = false
            }
        };

        public TextEditor() {
            this.Content                        = editor;
            editor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(editor.Options);

            this.SetHighlighter(EditorHighlighter.Default);
        }

        public void SetContent(String? source) {
            if(source == null) {
                editor.Text = "";
                return;
            }

            if(this.editor.SyntaxHighlighting == HighlightingManager.Instance.GetDefinition("XML")) {
                editor.Text = BeautifyXML(source);
                return;
            }

            editor.Text = source;
        }

        public void SetHighlighter(EditorHighlighter? type) {
            switch(type) {
                case EditorHighlighter.XML:
                    this.editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
                break;
                case EditorHighlighter.Default:
                    try {
                        using (Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FRED.UI.Syntax.Config.xshd")) {
                            if (stream == null) {
                                return;
                            }

                            using (XmlTextReader reader = new XmlTextReader(stream)) {
                                editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                            }
                        }
                    } catch (Exception e) {
                        /* Currently Do Nothing */
                        System.Diagnostics.Debug.Print(e.ToString());
                    }
                break;
            }
        }

        private String BeautifyXML(String source) {
            if(source.Trim().Length == 0) {
                return source;
            }

            String xml = source.Replace("\t", " ");
            xml = xml.Replace("\r", "");
            xml = xml.Replace("\n", " ");

            String formattedXml = Regex.Replace(xml, @"(</\w+>)", "$1\n");

            if(!Regex.IsMatch(formattedXml, @"^<\?xml(:\?)?[^>]*?>")) {
                formattedXml = "<" + TAG_ROOT + ">" + formattedXml;
            }
            
            formattedXml = Regex.Replace(formattedXml, @"^<\?xml(:\?)?[^>]*?>", "<?xml version=\"1.0\" encoding=\"utf-8\" ?><" + TAG_ROOT + ">\n");

            formattedXml += "</" + TAG_ROOT + ">";
            int indentation = 0;
            String[] lines = formattedXml.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            String result = "";

            foreach(String line in lines) {
                if(line.Contains("</")) {
                    indentation -= 1;
                }

                if(indentation > 0) {
                    result += new String('\t', indentation) + line + "\r\n";
                } else {
                    result += line + "\r\n";
                }

                if(line.Contains("<") && !line.Contains("</")) {
                    indentation += 1;
                }
            }

            try {
                return XDocument.Parse(CleanInvalidXmlChars(result.Trim())).ToString().Replace("<" + TAG_ROOT + ">", "").Replace("</" + TAG_ROOT + ">", "").Trim();
            } catch (Exception ex) {
                return "/** Fehler beim Formatieren des XML:\n" + ex.ToString() + " **/\r\n" + CleanInvalidXmlChars(result.Trim());
            }
        }
        private String CleanInvalidXmlChars(String xml) {
            return Regex.Replace(xml, @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]+", "");
        }
    }
}
