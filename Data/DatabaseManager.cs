using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;
using System.Threading.Tasks;

namespace Translation_Manager
{
    public static class TranslationDatabase
    {
        public static bool IsAutoSaveEnabled { get; set; }

        // create an empty database
        public static Dictionary<string, LineInfos> database = new Dictionary<string, LineInfos>();

        // Get info from App.config
        private static readonly string dataPath = ConfigurationManager.AppSettings.Get("DataPath");
        private static readonly string databaseFileName = ConfigurationManager.AppSettings.Get("databaseFileName");

        private static string fullDatabasePath = Path.Combine(dataPath, databaseFileName + ".json");


        #region Load Database
        internal static void LoadDatabase()
        {
            Log.Write("Loading Database...");
            if (File.Exists(fullDatabasePath))
            {
                database = JsonConvert.DeserializeObject<Dictionary<string, LineInfos>>(File.ReadAllText(fullDatabasePath));
            }
            //Log.Write("Database Loaded");
        }
        #endregion

        #region Save & Backup
        // save the database
        internal static async Task Save(bool backup = false)
        {
            MainWindow.canClickUI.CanClick = false;
            Log.Write("Saving Database, please wait...");
            if (backup)
            {
                string currentDate = DateTime.Now.ToString("yy.MM.dd-H.mm");
                fullDatabasePath = Path.Combine(dataPath, databaseFileName + " (Backup " + currentDate + ").json");
            }

            string databaseSavePath = fullDatabasePath;
            string json = await Task.Run(() => JsonConvert.SerializeObject(database, Formatting.Indented));
            await Task.Run(() => File.WriteAllText(databaseSavePath, json));
            Log.Write("Database Saved");
            MainWindow.canClickUI.CanClick = true;

            MainWindow.canClickUI.IsSaved = true;
        }
        #endregion

        #region Add database entry
        /// <summary>
        /// Add an entry to the database
        /// </summary>
        internal static void Add(string key, LineInfos lineInfos)
        {
            // If the key doesn't already exist, add the sent LineInfo object to the dictionary
            if (!database.ContainsKey(key))
            {
                database.Add(key, lineInfos);
            }

            MainWindow.canClickUI.IsSaved = false;
        }
        #endregion

        #region Update database entry
        /// <summary>
        /// Update the database making sure to replace nothing that might already exists
        /// </summary>
        internal static void Update(string key, LineInfos lineInfos)
        {
            if (database.ContainsKey(key))
            {
                if (string.IsNullOrEmpty(database[key].Japanese) && !string.IsNullOrEmpty(lineInfos.Japanese))
                {
                    database[key].Japanese = lineInfos.Japanese;
                }
                if (string.IsNullOrEmpty(database[key].Official) && !string.IsNullOrEmpty(lineInfos.Official))
                {
                    database[key].Official = lineInfos.Official;
                }
                if (string.IsNullOrEmpty(database[key].Deepl) && !string.IsNullOrEmpty(lineInfos.Deepl))
                {
                    database[key].Deepl = lineInfos.Deepl;
                }
                if (string.IsNullOrEmpty(database[key].Google) && !string.IsNullOrEmpty(lineInfos.Google))
                {
                    database[key].Google = lineInfos.Google;
                }
                if (lineInfos.InFile != null)
                {
                    database[key].InFile.AddRange(lineInfos.InFile);
                }
            }

            MainWindow.canClickUI.IsSaved = false;
        }
        #endregion

        #region Overwrite database entry
        // Unused
        internal static void Overwrite(string key, TlTypes tlType, string newString)
        {
            if (database.ContainsKey(key))
            {
                if (tlType == TlTypes.Official)
                {
                    database[key].Official = newString;
                }
                else if (tlType == TlTypes.DeepL)
                {
                    database[key].Deepl = newString;
                }
                else if (tlType == TlTypes.Google)
                {
                    database[key].Google = newString;
                }
            }

            MainWindow.canClickUI.IsSaved = false;
        }
        #endregion
    }
}
    