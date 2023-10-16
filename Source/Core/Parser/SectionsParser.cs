using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FRED.Core.Parser {
    public class Parser {
        private readonly String _content         = String.Empty;
        private GroupToken? _currentGroupToken   = null;
        private ValueToken? _currentToken        = null;
        private readonly List<IToken> _tokens    = new List<IToken>();


        public Parser(String content) {
            _content = content;
        }

        public void Parse() {
            int position           = 0;
            StringReader reader    = new StringReader(FixSingleLineGroups(_content));
            String? line           = String.Empty;

            while((line = reader.ReadLine()) != null) {
                ++position;

                if(String.IsNullOrEmpty(line)) {
                    continue;
                }

                ParseLine(line.Trim(), position);
            }
        }

        private void AddToken(IToken token) {
            if(_currentGroupToken != null) {
                _currentGroupToken.Tokens.Add(token);
            } else {
                _tokens.Add(token);
            }
        }

        private String? FixSingleLineGroups(String? value) {
            if(value == null) {
                return null;
            }

            return value.Replace(" { ", " {\n").Replace(" }", "\n}");
        }

        private String? TrimValue(String? value) {
            if(value == null) {
                return null;
            }

            return value.TrimStart('=').TrimEnd(';').Trim();
        }

        private void ParseLine(String text, int lineIndex) {
            int keyEndSymbol = text.IndexOf(' ');
            int endSymbol = text.IndexOf(';');
            int commendSymbol = text.IndexOf('#');
            int groupStartSymbol = text.IndexOf('{');
            int groupEndSymbol = text.IndexOf('}');

            if(text.Length == 0) {
                return;
            }

            switch(text[0]) {
                /* Comments */
                case '#':
                    AddToken(new CommentToken(text.Trim().TrimStart('#').Trim()));
                return;

                /* Values */
                case '\'':
                case '"':
                    if(_currentToken != null && _currentToken is ValueToken valueToken) {
                        valueToken.Value += TrimValue(text);

                        if(endSymbol > -1) {
                            valueToken.Value = TrimValue(valueToken.Value);
                            AddToken(valueToken);
                        }
                    } else {
                        throw new Exception("");
                    }
                return;

                /* Group End */
                case '}':
                    if(_currentGroupToken?.Parent != null) {
                        _currentGroupToken = _currentGroupToken?.Parent;
                    } else {
                        _currentGroupToken = null;
                    }

                    _currentToken = null;
                return;
            }

            string? key = null;
            string? value = null;
            string? commend = null;

            if(keyEndSymbol > -1) {
                key = text.Substring(0, keyEndSymbol);
            }

            if(groupStartSymbol > keyEndSymbol) {
                value = text.Substring(keyEndSymbol + 1, groupStartSymbol - keyEndSymbol - 1);
            } else if(endSymbol > keyEndSymbol) {
                value = text.Substring(keyEndSymbol + 1, endSymbol - keyEndSymbol - 1);
            } else if(endSymbol == -1) {
                value = text.Substring(keyEndSymbol + 1);
            }

            if(commendSymbol > keyEndSymbol) {
                commend = text.Substring(commendSymbol + 1).Trim().TrimStart('#').Trim();
            }

            if(groupStartSymbol > -1) {
                var groupToken = new GroupToken(_currentGroupToken, key, null, commend);
                AddToken(groupToken);
                _currentGroupToken = groupToken;
                return;
            }



            if(groupStartSymbol == -1 && endSymbol == -1) {
                _currentToken = new ValueToken(_currentGroupToken, key, TrimValue(value), commend);
                return;
            }

            if(endSymbol > -1) {
                AddToken(new ValueToken(_currentGroupToken, key, TrimValue(value), commend));
                return;
            }

            throw new Exception("");
        }

        public IReadOnlyList<IToken> GetTokens() {
            return _tokens;
        }
    }

    public class SectionsParser {
        private Parser _parser = null;
        private IList<IToken> _tokens = new List<IToken>();

        public void Parse(String content) {
            _parser = new Parser(content);
            _parser.Parse();
            _tokens = _parser.GetTokens().ToList();
        }

        public IToken GetList() {
            return _tokens[0];
        }

        public new String ToString() {
            return JsonConvert.SerializeObject(this.GetList(), Formatting.Indented, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
