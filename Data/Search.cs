using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.IO;

namespace Translation_Manager
{
    internal class Search
    {
        #region Starts the database list with a few infos rather than the entire Database.
        internal static void Init()
        {
            LineInfosUpdate InitInfo1 = new LineInfosUpdate()
            {
                Japanese = "This list will always start empty, to avoid long startup time."
            };
            LineInfosUpdate InitInfo2 = new LineInfosUpdate()
            {
                Japanese = "Select the translation to display with the checkboxes."
            };
            LineInfosUpdate InitInfo3 = new LineInfosUpdate()
            {
                Japanese = "Use the search box above."
            };
            MainWindow.DatabaseList.Clear();
            MainWindow.DatabaseList.Add(InitInfo1);
            MainWindow.DatabaseList.Add(InitInfo2);
            MainWindow.DatabaseList.Add(InitInfo3);
        }
        #endregion

        #region Search in Database
        /// <summary>
        /// Search the given string in the database
        /// </summary>
        /// <param name="searchedText"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal static async Task String(string searchedText, TlTypes mode, IProgress<int> progress, CancellationToken cancellationToken)
        {
            MainWindow.DatabaseList.Clear();

            // Progress bar implementation.
            int maxCount = TranslationDatabase.database.Count();
            int count = 0;
            int lastpercent = 0;
            progress.Report(count);


            if (mode == TlTypes.Japanese)
            {
                foreach (KeyValuePair<string, LineInfos> keyValue in TranslationDatabase.database)
                {
                    if (keyValue.Value.Japanese.Contains(searchedText))
                    {
                        LineInfosUpdate lineInfosUpdate = GetLineInfosUpdate(keyValue.Value);

                        await Task.Run(() =>
                        App.Current.Dispatcher.Invoke(() => {
                            MainWindow.DatabaseList.Add(lineInfosUpdate);
                        }));
                    }

                    // Progress bar implementation.
                    count += 1;

                    int percentProgression = count * 100 / maxCount;
                    if (percentProgression >= lastpercent + 1)
                    {
                        progress.Report(percentProgression);
                        lastpercent = percentProgression;
                    }

                    // Cancellation Implementaiton
                    if (cancellationToken.IsCancellationRequested)
                    {
                        progress.Report(0);
                        break;
                    }
                }
            }
            else if ( mode == TlTypes.Official)
            {
                foreach (KeyValuePair<string, LineInfos> keyValue in TranslationDatabase.database)
                {
                    if (keyValue.Value.Official.Contains(searchedText))
                    {
                        LineInfosUpdate lineInfosUpdate = GetLineInfosUpdate(keyValue.Value);

                        await Task.Run(() =>
                        App.Current.Dispatcher.Invoke(() => {
                            MainWindow.DatabaseList.Add(lineInfosUpdate);
                        }));
                    }

                    // Progress bar implementation.
                    count += 1;

                    int percentProgression = count * 100 / maxCount;
                    if (percentProgression >= lastpercent + 1)
                    {
                        progress.Report(percentProgression);
                        lastpercent = percentProgression;
                    }

                    // Cancellation Implementaiton
                    if (cancellationToken.IsCancellationRequested)
                    {
                        progress.Report(0);
                        break;
                    }
                }
            }
            else if (mode == TlTypes.DeepL)
            {
                foreach (KeyValuePair<string, LineInfos> keyValue in TranslationDatabase.database)
                {
                    if (keyValue.Value.Deepl.Contains(searchedText))
                    {
                        LineInfosUpdate lineInfosUpdate = GetLineInfosUpdate(keyValue.Value);

                        await Task.Run(() =>
                        App.Current.Dispatcher.Invoke(() => {
                            MainWindow.DatabaseList.Add(lineInfosUpdate);
                        }));
                    }

                    // Progress bar implementation.
                    count += 1;

                    int percentProgression = count * 100 / maxCount;
                    if (percentProgression >= lastpercent + 1)
                    {
                        progress.Report(percentProgression);
                        lastpercent = percentProgression;
                    }

                    // Cancellation Implementaiton
                    if (cancellationToken.IsCancellationRequested)
                    {
                        progress.Report(0);
                        break;
                    }
                }
            }
            else if (mode == TlTypes.Google)
            {
                foreach (KeyValuePair<string, LineInfos> keyValue in TranslationDatabase.database)
                {
                    if (keyValue.Value.Google.Contains(searchedText))
                    {
                        LineInfosUpdate lineInfosUpdate = GetLineInfosUpdate(keyValue.Value);

                        await Task.Run(() =>
                        App.Current.Dispatcher.Invoke(() => {
                            MainWindow.DatabaseList.Add(lineInfosUpdate);
                        }));
                    }

                    // Progress bar implementation.
                    count += 1;

                    int percentProgression = count * 100 / maxCount;
                    if (percentProgression >= lastpercent + 1)
                    {
                        progress.Report(percentProgression);
                        lastpercent = percentProgression;
                    }

                    // Cancellation Implementaiton
                    if (cancellationToken.IsCancellationRequested)
                    {
                        progress.Report(0);
                        break;
                    }
                }
            }
        }
        #endregion

        #region Search Suspicious translations
        internal static async Task Suspicious(bool isDeepl, IProgress<int> progress, CancellationToken cancellationToken)
        {
            string dataPath = ConfigurationManager.AppSettings.Get("dataPath");
            string listFile = ConfigurationManager.AppSettings.Get("suspiciousTranslationFile");
            string listFilePath = Path.Combine(dataPath, listFile);
            string[] suspiciousArray;
            List<string> suspiciousList = new List<string>();
            int suspiciousCount = 0;

            // Progress bar implementation.
            int maxCount = TranslationDatabase.database.Count();
            int count = 0;
            int lastpercent = 0;
            progress.Report(count);

            MainWindow.DatabaseList.Clear();

            // Load all the suspicious string to search for.
            if (File.Exists(listFilePath))
            {
                suspiciousArray = File.ReadAllLines(listFilePath);
            }
            else
            {
                Log.Write("Can't Find the suspicious list.");
                return;
            }

            // remove commented lines.
            foreach (string item in suspiciousArray)
            {
                if (!item.StartsWith("//")) { suspiciousList.Add(item); }
            }

            // Look for them in the dabatase.
            foreach (KeyValuePair<string, LineInfos> keyValuePair in TranslationDatabase.database)
            {
                if (suspiciousList.Any(keyValuePair.Value.Deepl.Contains) && isDeepl)
                {
                    LineInfosUpdate lineInfosUpdate = GetLineInfosUpdate(keyValuePair.Value);

                    await Task.Run(() =>
                    App.Current.Dispatcher.Invoke(() => {
                        MainWindow.DatabaseList.Add(lineInfosUpdate);
                    }));

                    suspiciousCount += 1;
                }
                else if (suspiciousList.Any(keyValuePair.Value.Google.Contains) && !isDeepl)
                {
                    LineInfosUpdate lineInfosUpdate = GetLineInfosUpdate(keyValuePair.Value);

                    await Task.Run(() =>
                    App.Current.Dispatcher.Invoke(() => {
                        MainWindow.DatabaseList.Add(lineInfosUpdate);
                    }));

                    suspiciousCount += 1;
                }

                // Progress bar implementation.
                count += 1;

                int percentProgression = count * 100 / maxCount;
                if (percentProgression >= lastpercent + 1)
                {
                    progress.Report(percentProgression);
                    lastpercent = percentProgression;
                }

                // Cancellation Implementaiton
                if (cancellationToken.IsCancellationRequested)
                {
                    progress.Report(0);
                    break;
                }
            }
            Log.Write(suspiciousCount + " suspicious translations found");
        }

        #endregion

        private static LineInfosUpdate GetLineInfosUpdate(LineInfos lineInfos)
        {
            LineInfosUpdate lineInfosUpdate = new LineInfosUpdate()
            {
                HashValue = lineInfos.HashValue,
                Japanese = lineInfos.Japanese,
                Official = lineInfos.Official,
                Deepl = lineInfos.Deepl,
                Google = lineInfos.Google
            };
            lineInfosUpdate.IsInit = true;
            return lineInfosUpdate;
        }
    }
}
