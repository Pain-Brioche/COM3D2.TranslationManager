using System.Collections.Generic;

namespace Translation_Manager
{
    public class LineInfos
    {
        public string HashValue { get; set; }
        public string Japanese { get; set; }
        public string Official { get; set; }
        public string Deepl { get; set; }
        public string Google { get; set; }
        public List<string> InFile { get; set; }
    }

    public class LineInfosUpdate : LineInfos
    {
        private string _official;
        private string _deepl;
        private string _google;
        public bool IsInit { get; set; } = false;

        public new string Official {
            get => _official;
            set {
                if (_official == value) { return; }

                _official = value;

                if (IsInit)
                {
                    TranslationDatabase.Overwrite(HashValue, TlTypes.Official, value);
                }
            }
        }

        public new string Deepl {
            get => _deepl;
            set {
                if (_deepl == value) { return; }

                _deepl = value;

                if (IsInit)
                {
                    TranslationDatabase.Overwrite(HashValue, TlTypes.DeepL, value);
                }
            }
        }

        public new string Google {
            get => _google;
            set {
                if (_google == value) { return; }

                _google = value;

                if (IsInit)
                {
                    TranslationDatabase.Overwrite(HashValue, TlTypes.Google, value);
                }
            }
        }
    }
}
