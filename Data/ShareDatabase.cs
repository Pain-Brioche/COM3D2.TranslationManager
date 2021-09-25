using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Translation_Manager
{
    internal class ShareDatabase
    {
        #region Export Database to share with others
        /// <summary>
        /// Export Database with chosen translations
        /// </summary>
        /// <param name="deepl"></param>
        /// <param name="google"></param>
        /// <param name="progress"></param>
        internal void Export(bool deepl, bool google, IProgress<int> progress)
        {
            Log.Write("Exporting Database, please wait...");
            string exportPath = ConfigurationManager.AppSettings.Get("exportPath");

            // Progress bar implementation.
            int maxCount = TranslationDatabase.database.Count();
            int count = 0;
            int lastpercent = 0;
            progress.Report(count);

            // duplicate current database
            string jsonTemp = JsonConvert.SerializeObject(TranslationDatabase.database);
            progress.Report(10);
            Dictionary<string, LineInfos> exportDatabase = JsonConvert.DeserializeObject<Dictionary<string, LineInfos>>(jsonTemp);
            progress.Report(20);
            lastpercent = 20;

            foreach (KeyValuePair<string, LineInfos> keyValuePair in exportDatabase)
            {
                //remove any unwantend entries
                keyValuePair.Value.Official = null;
                if (!deepl) { keyValuePair.Value.Deepl = null; }
                if (!google) { keyValuePair.Value.Google = null; }

                // Progress bar implementation.
                count += 1;

                int percentProgression = count * 100 / maxCount;
                if (percentProgression >= lastpercent + 1)
                {
                    progress.Report(percentProgression);
                    lastpercent = percentProgression;
                }
            }

            // exported file name.
            string currentDate = DateTime.Now.ToString("yy.MM.dd-H.mm");
            string exportName = "SharedDatabase " + currentDate + " (JP";
            if (deepl) { exportName += "_DpL"; }
            if (google) { exportName += "_Ggl"; }
            exportName += ").json";
            string exportFullPath = Path.Combine(exportPath, exportName);

            string json = JsonConvert.SerializeObject(exportDatabase, Formatting.Indented);
            File.WriteAllText(exportFullPath, json);

            Log.Write("Export done: " + exportName);
        }
        #endregion

        #region Import Shared Database
        /// <summary>
        /// Import Database
        /// </summary>
        /// <param name="isJapaneseImported"></param>
        /// <param name="isDeeplImported"></param>
        /// <param name="isGoogleImported"></param>
        /// <param name="overwrite"></param>
        /// <param name="path"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        internal async Task Import(bool isJapaneseImported, bool isDeeplImported, bool isGoogleImported, bool overwrite, string path, IProgress<int> progress)
        {
            Dictionary<string, LineInfos> importedDatabase = JsonConvert.DeserializeObject<Dictionary<string, LineInfos>>(File.ReadAllText(path));

            // Progress bar implementation.
            int maxCount = importedDatabase.Count();
            int count = 0;
            int lastpercent = 0;
            progress.Report(count);

            foreach (KeyValuePair<string, LineInfos> keyValuePair in importedDatabase)
            {
                if (isJapaneseImported)
                {
                    if (!TranslationDatabase.database.ContainsKey(keyValuePair.Key))
                    {
                        await Task.Run(() => TranslationDatabase.database.Add(keyValuePair.Key, keyValuePair.Value));
                        continue;
                    }
                }
                if (TranslationDatabase.database.ContainsKey(keyValuePair.Key))
                {
                    if (overwrite)
                    {
                        if (isDeeplImported) { await Task.Run(() => TranslationDatabase.Overwrite(keyValuePair.Key, TlTypes.DeepL, keyValuePair.Value.Deepl)); }
                        if (isGoogleImported) { await Task.Run(() => TranslationDatabase.Overwrite(keyValuePair.Key, TlTypes.Google, keyValuePair.Value.Google)); }
                    }
                    else
                    {
                        if (isDeeplImported) { await Task.Run(() => TranslationDatabase.Update(keyValuePair.Key, keyValuePair.Value)); }
                        if (isGoogleImported) { await Task.Run(() => TranslationDatabase.Update(keyValuePair.Key, keyValuePair.Value)); }
                    }
                }

                // Progress bar implementation.
                count += 1;
                
                int percentProgression = count * 100 / maxCount;
                if (percentProgression >= lastpercent + 1)
                {
                    progress.Report(percentProgression);
                    lastpercent = percentProgression;
                }
            }
            
            if (TranslationDatabase.IsAutoSaveEnabled)
            {
                TranslationDatabase.Save();
            }
        }
        #endregion
    }
}
