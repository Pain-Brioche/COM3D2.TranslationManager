using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Translation_Manager
{
    internal class Sort
    {

        /// <summary>
        /// Sort a folder according to their prefix.
        /// </summary>
        /// <param name="path"></param>
        internal static void Scripts(string path, IProgress<int> progress)
        {
            // Get info from App.config
            string dataPath = ConfigurationManager.AppSettings.Get("DataPath");
            string sortFoldersFileName = ConfigurationManager.AppSettings.Get("SortFolderFile");
            string fullSortFolderFilePath = Path.Combine(dataPath, sortFoldersFileName);

            // Load sort file dictionary
            Dictionary<string, string> sortDict = new Dictionary<string, string>();

            if (File.Exists(fullSortFolderFilePath))
            {
                sortDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(fullSortFolderFilePath));
            }
            else
            {
                Log.Write(sortFoldersFileName + " is missing.");
                return;
            }

            // Make folders to sort files in
            foreach (KeyValuePair<string, string> keyValuePair in sortDict)
            {
                Tools.MakeFolder(Path.Combine(path, keyValuePair.Value));
            }
            Tools.MakeFolder(Path.Combine(path, "[UnCategorized]"));

            // get all .txt files 
            string[] fileList = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories);

            // setting progress bar
            int maxCount = fileList.Length;
            int count = 0;
            int percentProgress = 0;
            progress.Report(percentProgress);
  
            // move files according to their names.
            foreach (string file in fileList)
            {
                string fileName = Path.GetFileName(file);
                bool isSorted = false;

                foreach (KeyValuePair<string, string> keyValuePair in sortDict)
                {
                    if (fileName.StartsWith(keyValuePair.Key))
                    {
                        string fileSortedPath = Path.Combine(path, keyValuePair.Value, fileName);

                        if (File.Exists(fileSortedPath))
                        {
                            if (file != fileSortedPath)
                            {
                                List<string> missingLines = MergeScripts(fileSortedPath, file);
                                File.AppendAllLines(fileSortedPath, missingLines);
                                File.Delete(file);
                            }
                        }
                        else
                        {
                            File.Move(file, fileSortedPath);
                        }
                        isSorted = true;
                        break;
                    }
                }

                if (!isSorted)
                {
                    string fileSortedPath = Path.Combine(path, "[UnCategorized]", fileName);

                    if (File.Exists(fileSortedPath))
                    {
                        if (file != fileSortedPath)
                        {
                            List<string> missingLines = MergeScripts(fileSortedPath, file);
                            File.AppendAllLines(fileSortedPath, missingLines);
                            File.Delete(file);
                        }
                    }
                    else
                    {
                        File.Move(file, fileSortedPath);
                    }
                }

                count += 1;
                percentProgress = count * 100 / maxCount;
                progress.Report(percentProgress);
            }

            DeleteDirectory(path);
        }

        /// <summary>
        /// Merge Scripts with the same file name
        /// </summary>
        /// <param name="fileOne"></param>
        /// <param name="fileTwo"></param>
        /// <returns></returns>
        private static List<string> MergeScripts(string fileOne, string fileTwo)
        {
            List<string> lineListOne = File.ReadAllLines(fileOne).ToList();
            List<string> lineListTwo = File.ReadAllLines(fileTwo).ToList();
            List<string> endList = new List<string>();

            foreach (string line in lineListTwo)
            {
                if (!lineListOne.Contains(line))
                {
                    endList.Add(line);
                }
            }
            return endList;
        }

        /// <summary>
        /// Delete empty directories recursively
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteDirectory(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);

                if (Directory.GetFileSystemEntries(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}
