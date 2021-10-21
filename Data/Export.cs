using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Configuration;

namespace Translation_Manager
{
    public static class Export
    {
        #region Export to translate using Google Sheet
        /// <summary>
        /// Export to CSV for translation outside of the program.
        /// Using GoogleSheet.
        /// </summary>
        public static void ToTranslateGoogle()
        {
            string exportPath = ConfigurationManager.AppSettings.Get("exportPath");
            string GoogleCsv = ConfigurationManager.AppSettings.Get("ToTranslateCsv");
            string exportGoogleCsvPath = Path.Combine(exportPath, GoogleCsv);

            int lineCount = 0;

            List<string> outList = new List<string>
            {
                "Key|Translation|Japanese"
            };

            // make a list containing all lines we want to translate
            foreach (KeyValuePair<string, LineInfos> entry in TranslationDatabase.database)
            {
                // Check if there is no translations already available
                if (string.IsNullOrEmpty(entry.Value.Official) && string.IsNullOrEmpty(entry.Value.Deepl) && string.IsNullOrEmpty(entry.Value.Google))
                {
                    lineCount += 1;
                    //Adds the translation instruction for google sheet inside the csv
                    string googleSheetTranslationInstruction = "=GOOGLETRANSLATE(C" + (lineCount + 1).ToString() + "; \"auto\"; \"en\")";
                    outList.Add(entry.Key + "|" + googleSheetTranslationInstruction + "|" + entry.Value.Japanese);

                }
            }
            File.WriteAllLines(exportGoogleCsvPath, outList, Encoding.UTF8);
            Log.Write(lineCount.ToString() + " Lines to translate exported.");
        }
        #endregion

        #region Export to translate using DeepL Online service
        /// <summary>
        /// Export to TXT for translation outside of the program.
        /// Using DeepL.com
        /// </summary>

        public static void ToTranslateDeepL(bool isSplitEnable, int characterLimit = 0)
        {
            string exportPath = ConfigurationManager.AppSettings.Get("exportPath");

            int lineCount = 0;

            List<string> outList = new List<string>();

            // make a list containing all lines we want to translate
            foreach (KeyValuePair<string, LineInfos> entry in TranslationDatabase.database)
            {
                // for deepl we want to avoid translating lines with official translation or already having deepl
                if (entry.Value.Official == "" && entry.Value.Deepl == "")
                {
                    outList.Add(entry.Key + "|" + entry.Value.Japanese);
                    lineCount += 1;
                }
            }

            if (isSplitEnable)
            {
                int fileCount = 1;
                int characterCount = 0;
                string splitTxt = ConfigurationManager.AppSettings.Get("SplitTxt");
                string splitFilePath = Path.Combine(exportPath, splitTxt + fileCount + ".txt");

                // delete eventual old split files
                DirectoryInfo di = new DirectoryInfo(exportPath);
                foreach (FileInfo file in di.GetFiles("*" + splitTxt + "*"))
                {
                    file.Delete();
                }

                // Increment by one if the limit is reached and add the line to a file
                foreach (string l in outList)
                {
                    string line = l + "\n";
                    int lCount = line.Length;
                    if (characterCount + lCount > characterLimit)
                    {
                        fileCount += 1;
                        characterCount = lCount;
                        splitFilePath = Path.Combine(exportPath, splitTxt + fileCount + ".txt");
                    }
                    else
                    {
                        characterCount += lCount;
                    }
                    File.AppendAllText(splitFilePath, line, Encoding.UTF8);
                }
                Log.Write(lineCount.ToString() + " Lines to translate exported, in " + fileCount + " files.");
            }
            else
            {
                File.WriteAllLines(Path.Combine(exportPath, "ToTranslate.txt"), outList, Encoding.UTF8);
                Log.Write(lineCount.ToString() + " Lines to translate exported.");
            }
        }
        #endregion

        #region Export to i18nEx format
        /// <summary>
        /// Export the database as an i18nEx compatible file, choosing the best available translations.
        /// </summary>
        public static void ToI18nEx()
        {
            Log.Write("Exporting as i18nEx, this can take a while, wait for the end message");

            int lineCount = 0;
            int fileCount = 0;
            string exportI18Path = ConfigurationManager.AppSettings.Get("ExportI18Path");

            //delete existing files to avoid adding to already exported files or outdated ones.
            if (Directory.Exists(exportI18Path))
            {
                Directory.Delete(exportI18Path, true);
            }
            
            //create i18nEx folder tree
            Tools.MakeFolder(exportI18Path);

            // create a dictionary to store each line a file has Key = fileName, Value = List<endString>
            Dictionary<string, List<string>> i18Dict = new Dictionary<string, List<string>>();


            // go through the database
            foreach (KeyValuePair<string, LineInfos> keyValue in TranslationDatabase.database)
            {
                string bestTranslation;

                // choose the best translation Official > DeepL > Google
                if (keyValue.Value.Official != "") { bestTranslation = keyValue.Value.Official; }
                else if (keyValue.Value.Deepl != "") { bestTranslation = keyValue.Value.Deepl; }
                else { bestTranslation = keyValue.Value.Google; }

                lineCount += 1;

                // make a i18nEx compatible string and add it to i18Dict
                string endString = keyValue.Value.Japanese + "\t" + bestTranslation;

                foreach (string file in keyValue.Value.InFile)
                {
                    if (!i18Dict.ContainsKey(file))
                    {
                        List<string> stringList = new List<string>() { endString };
                        i18Dict.Add(file, stringList);
                    }
                    else
                    {
                        i18Dict[file].Add(endString);
                    }
                }
            }

            // create every files from i18Dict
            foreach (KeyValuePair<string, List<string>> keyValuePair in i18Dict)
            {
                fileCount += 1;
                string filePath = exportI18Path + keyValuePair.Key;
                File.WriteAllLines(filePath, keyValuePair.Value);
            }

            // sort the resulting files.
            //Sort.Scripts(exportI18Path);

            Log.Write(lineCount + " lines exported in " + fileCount + " files.");
        }
        #endregion

        #region Export to XUAT format
        /// <summary>
        /// Export the database as a XUAT compatible file, choosing the best available translations.
        /// </summary>
        public static void ToXuat()
        {
            int lineCount = 0;
            string exportXuatPath = ConfigurationManager.AppSettings.Get("XuatFile");
            List<string> xuatList = new List<string>();

            foreach (KeyValuePair<string, LineInfos> keyValue in TranslationDatabase.database)
            {
                string bestTranslation;

                // choose the best translation Official > DeepL > Google (ignore if no translation)
                if (keyValue.Value.Official != "") { bestTranslation = keyValue.Value.Official; }
                else if(keyValue.Value.Deepl != "") { bestTranslation = keyValue.Value.Deepl; }
                else if (keyValue.Value.Google != "") { bestTranslation = keyValue.Value.Google; }
                else { continue;  }

                lineCount += 1;

                // make a XUAT compatible string
                string endString = keyValue.Value.Japanese + "=" + bestTranslation;
                xuatList.Add(endString);
            }
            File.WriteAllLines(exportXuatPath, xuatList);
            Log.Write(lineCount + " lines exported.");
            Log.Write("XUAT_Translations.txt has been created alongside the program.");
            Log.Write(@"Place it in your XUAT translation folder (BepinEx\Translation\en\Text).");
        }
        #endregion        
    }
}
