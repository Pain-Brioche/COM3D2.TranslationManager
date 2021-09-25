using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translation_Manager
{
    public class StatusBar : INotifyPropertyChanged
    {
        private int totalLines = 0;
        private int official = 0;
        private int deepl = 0;
        private int google = 0;
        private int remaining = 0;
        private int totalTranslated = 0;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Properties
        // Self aware custom properties
        public int TotalLines {
            get => totalLines;
            set {
                if (totalLines == value) { return; }

                totalLines = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(TotalLines)));
            }
        }
        public int Official {
            get => official;
            set {
                if (official == value) { return; }

                official = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Official)));
            }
        }
        public int DeepL {
            get => deepl;
            set {
                if (deepl == value) { return; }

                deepl = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(DeepL)));
            }
        }
        public int Google {
            get => google;
            set {
                if (google == value) { return; }

                google = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Google)));
            }
        }
        public int Remaining {
            get => remaining;
            set {
                if (remaining == value) { return; }

                remaining = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Remaining)));
            }
        }
        public int TotalTranslated {
            get => totalTranslated;
            set {
                if (totalTranslated == value) { return; }

                totalTranslated = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(TotalTranslated)));
            }
        }
        #endregion

        /// <summary>
        /// Update stats status bar display.
        /// </summary>
        internal void UpdateStats()
        {
            Log.Write("Updating Stats...");

            totalLines = official = deepl = google = remaining = totalTranslated = 0;

            TotalLines = TranslationDatabase.database.Count();

            foreach (KeyValuePair<string, LineInfos> keyValue in TranslationDatabase.database)
            {
                bool isTranslated = false;
                if (!string.IsNullOrEmpty(keyValue.Value.Official)) { Official += 1; isTranslated = true; }
                if (!string.IsNullOrEmpty(keyValue.Value.Deepl)) { DeepL += 1; isTranslated = true; }
                if (!string.IsNullOrEmpty(keyValue.Value.Google)) { Google += 1; isTranslated = true; }
                if (isTranslated) { TotalTranslated += 1; }
            }
            Remaining = TotalLines - TotalTranslated;
        }
    }
}
