using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Translation_Manager
{
    internal static class ImportData
    {
        #region Import from script
        /// <summary>
        /// Import the file in the selected path inside the database
        /// </summary>
        internal static async Task ImportScript(TlTypes mode, string path, IProgress<int> progress)
        {
            int fileCount = 0;
            int lineCount = 0;

            // Get all .txt files in the sent directory and its subfolders
            string[] fileList = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);

            foreach (string file in fileList)
            {
                // Returns an array of all lines in the file.
                string[] lineList = File.ReadAllLines(file);

                // returns the file name from a path
                string inFile = Path.GetFileName(file);

                foreach (string line in lineList)
                {

                    // avoid empty lines
                    if (line == "") { continue; }

                    string hashValue, officialString, deeplString, googleString;
                    officialString = deeplString = googleString = "";

                    string trimLine = line.Trim();
                    // splits the line at each "tab" '\t'
                    string[] splitList = trimLine.Split('\t');
                    // removes leading and trailing whitespace (including tab)
                    for (int i = 0; i < splitList.Length; i++)
                    {
                        splitList[i] = splitList[i].Trim();
                    }

                    // Consider anormal to have anything other than 1 element if the mode in Japanese, so skip.
                    if (mode == TlTypes.Japanese && splitList.Length != 1) { continue; }

                    if (mode != TlTypes.Japanese)
                    {
                        // Consider anormal to have anything other than 2 elements in the splitList if the mode isn't Japanese, so skip.
                        if (splitList.Length != 2) { continue; }

                        // assign variable to the second part of the list depending on the mode
                        if (mode == TlTypes.Official) { officialString = splitList[1]; }
                        else if (mode == TlTypes.DeepL) { deeplString = splitList[1]; }
                        else if (mode == TlTypes.Google) { googleString = splitList[1]; }
                    }

                    //create MD5 hash from the first part which is always Japanese
                    hashValue = Tools.GetMd5(splitList[0]);

                    // create the LineInfos object to be send to TranslationDatabase.Update
                    LineInfos lineInfos = new LineInfos()
                    {
                        HashValue = hashValue,
                        Japanese = splitList[0],
                        Official = officialString,
                        Deepl = deeplString,
                        Google = googleString,
                        InFile = new List<string>() { inFile }
                    };

                    lineCount += 1;
                    if (TranslationDatabase.database.ContainsKey(hashValue))
                    {
                        TranslationDatabase.Update(hashValue, lineInfos);
                    }
                    else
                    {
                        TranslationDatabase.Add(hashValue, lineInfos);
                    }
                }

                fileCount += 1;

                // Progress bar implementation.
                int percentProgression = fileCount * 100 / fileList.Length;
                progress.Report(percentProgression);

            }
            Log.Write("Updated/Added " + lineCount + " entries from " + fileCount + " files.");

            if (TranslationDatabase.IsAutoSaveEnabled) { await TranslationDatabase.Save(); }
        }
        #endregion

        #region Import from GoogleSheet result
        internal static void ImportGoogle(string path)
        {
            int lineCount = 0;
            int invalidLineCount = 0;

            string[] fileList = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);

            foreach (string file in fileList)
            {
                // Returns an array of all lines in the file.
                string[] lineList = File.ReadAllLines(file);

                foreach (string line in lineList) 
                {
                    // Googlesheet will save strings with commas inside doublequotes ""
                    // replacing them with |

                    string curatedLine = line.Replace("\",", "|");
                    curatedLine = curatedLine.Replace(",\"", "|");

                    // split them according to the two possible delimiters , or |
                    string[] comaSplit = curatedLine.Split(',');
                    string[] pipeSplit = curatedLine.Split('|');

                    string[] endSplit;
                    // consider the array that gives three elements as a result as valid, otherwise skip this line.
                    if (pipeSplit.Length == 3) { endSplit = pipeSplit; }
                    else if (comaSplit.Length == 3) { endSplit = comaSplit; }
                    else { invalidLineCount += 1; continue; }

                    LineInfos lineInfos = new LineInfos()
                    {
                        Google = endSplit[1]
                    };
                    lineCount += 1;
                    TranslationDatabase.Update(endSplit[0], lineInfos);
                }
            }
            Log.Write(lineCount + " line(s) imported as Google");
            Log.Write(invalidLineCount + " line(s) flagged as Invalid and ignored");

            if (TranslationDatabase.IsAutoSaveEnabled) { TranslationDatabase.Save(); }
        }
        #endregion

        #region Import from DeepL result.
        internal static void ImportDeepl(string path)
        {
            int lineCount = 0;
            int invalidLineCount = 0;

            string[] fileList = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);

            foreach (string file in fileList)
            {
                // Returns an array of all lines in the file.
                string[] lineList = File.ReadAllLines(file);

                foreach (string line in lineList)
                {
                    string[] splitLine = line.Split('|');
                    // ignore lines that returns anything but two elements after split. 
                    if (splitLine.Length != 2) { invalidLineCount += 1; continue; }

                    lineCount += 1;

                    splitLine[1] = splitLine[1].Trim();

                    LineInfos lineInfos = new LineInfos() { Deepl = splitLine[1] };
                    TranslationDatabase.Update(splitLine[0], lineInfos);
                }
            }
            Log.Write(lineCount + " line(s) imported as DeepL");
            Log.Write(invalidLineCount + " line(s) flagged as Invalid and ignored");

            if (TranslationDatabase.IsAutoSaveEnabled) { TranslationDatabase.Save(); }
        }
        #endregion
    }
}
