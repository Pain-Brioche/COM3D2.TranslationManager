using System;
using System.Windows;
using System.Collections.ObjectModel;
using WinForm = System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
// using Microsoft.Win32;

namespace Translation_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal WinForm.FolderBrowserDialog folderBrowserDialog = new WinForm.FolderBrowserDialog();

        internal static ObservableCollection<string> LogEntries = new ObservableCollection<string>();

        internal static StatusBar statusBar = new StatusBar();

        // Cancelation implementation
        internal CancellationTokenSource cancelSearch = new CancellationTokenSource();

        internal static ObservableCollection<LineInfosUpdate> DatabaseList = new ObservableCollection<LineInfosUpdate>();

        internal static CanClickUI canClickUI = new CanClickUI();

        // Executed when the program starts
        public MainWindow()
        {
            InitializeComponent();

            // Various data Binding
            StatusBarDockPanel.DataContext = statusBar;

            //StatusBarProgressBar.DataContext = progressBar;

            RightPanelLog.ItemsSource = LogEntries;

            RightPanelListView.ItemsSource = DatabaseList;

            //disable buttons when Database is working
            ImportScriptImportButton.DataContext =
            ImportCsvImportButton.DataContext =
            ExportCsvExportButton.DataContext =
            ExportToGameButton.DataContext =
            DatabaseSaveButton.DataContext =
            DatabaseBackupButton.DataContext =
            DatabaseShareButton.DataContext =
            RefreshDatabaseStatsButton.DataContext =
            SearchButton.DataContext =
            DeleteListButton.DataContext =
            ReplaceStringButton.DataContext = canClickUI;
        }

        // executed when the UI is fully loaded (async)
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Write("Please Wait...");
            Tools.MakeFolder("Data");
            Tools.MakeFolder("Export");
            Search.Init();
            await Task.Run(() => TranslationDatabase.LoadDatabase());
            await Task.Run(() => statusBar.UpdateStats());
            canClickUI.CanClick = true;
            Log.Write("Manager Ready!");
            Log.Write("Dealing with thousands of text files takes a lot if time, be patient!");
        }

        #region Import from Scripts Buttons
        private void ImportScriptBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == WinForm.DialogResult.OK)
            {
                ImportScriptPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void ImportScriptButton_Click(object sender, RoutedEventArgs e)
        {
            TlTypes mode = Tools.GetTlType(ImportScriptComboBox.Text);
            string path = this.ImportScriptPathTextBox.Text;
            if (mode == TlTypes.Invalid)
            {
                Log.Write("Please select the type of script imported.");
                return;
            }
            if (path == "")
            {
                Log.Write("Please select a folder to import scripts from.");
                return;
            }

            canClickUI.CanClick = false;
            // Progress bar implementation.
            Progress<int> progress = new Progress<int>(value => {
                StatusBarProgressBar.Value = value;
            });

            await Task.Run(() => ImportData.ImportScript(mode, path, progress));
            canClickUI.CanClick = true;
        }
        #endregion

        #region Import/Export Google & DeepL Buttons
        private void ImportCsvBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == WinForm.DialogResult.OK)
            {
                this.ImportCsvTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void ExportCsvExportButton_Click(object sender, RoutedEventArgs e)
        {
            string mode = ExportCsvComboBox.Text;

            if (mode == "Export As")
            {
                Log.Write("Please select the type of Export");
            }
            else if (mode == "CSV For GoogleSheet")
            {
                Export.ToTranslateGoogle();
            }
            else if (mode == "TXT for DeepL")
            {
                bool isSplitEnable = (bool)EnableSplitCheckBox.IsChecked;
                string splitLimit = EnableSplitTextBox.Text;

                // check if the string can be converted as int
                if (isSplitEnable)
                {
                    if (int.TryParse(splitLimit, out int characterLimit))
                    {
                        Export.ToTranslateDeepL(true, characterLimit);
                    }
                    else
                    {
                        Log.Write("Maximum Characters must be a valid number");
                    }
                }
                else
                {
                    Export.ToTranslateDeepL(false);
                }
            }
        }

        private void ImportCsvImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImportCsvTextBox.Text == "")
            {
                Log.Write("Please select a folder to import from");
            }
            else if (ImportCsvComboBox.Text == "GoogleSheet CSV")
            {
                ImportData.ImportGoogle(ImportCsvTextBox.Text);
            }
            else if (ImportCsvComboBox.Text == "DeepL TXT")
            {
                ImportData.ImportDeepl(ImportCsvTextBox.Text);
            }
            else { Log.Write("Please select the type of translation imported."); }
        }
        #endregion

        #region Export to COM Button
        private async void ExportToGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExportToGameComboBox.Text == "i18nEx") { await Task.Run(() => Export.ToI18nEx()); }
            else { await Task.Run(() => Export.ToXuat()); }
        }
        #endregion

        #region Switch RightPanel Mode Buttons

        // Change the right panel from Log to Dabatase when clicking the radio buttons
        private void RadioButtonLog_Click(object sender, RoutedEventArgs e)
        {
            RightPanelDatabase.Visibility = Visibility.Hidden;
            RightPanelLog.Visibility = Visibility.Visible;
        }

        private void RadioButtonDatabase_Click(object sender, RoutedEventArgs e)
        {
            RightPanelLog.Visibility = Visibility.Hidden;
            RightPanelDatabase.Visibility = Visibility.Visible;
        }
        #endregion

        #region Database display
        private void OfficialCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)OfficialCheckBox.IsChecked)
            {
                OfficialHeader.IsEnabled = true;
                OfficialColumn.Width = 460;
            }
            else
            {
                OfficialHeader.IsEnabled = false; ;
                OfficialColumn.Width = 0;
            }
        }

        private void DeepLCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)DeepLCheckBox.IsChecked)
            {
                DeepLHeader.IsEnabled = true;
                DeepLColumn.Width = 460;
            }
            else
            {
                DeepLHeader.IsEnabled = false; ;
                DeepLColumn.Width = 0;
            }
        }

        private void GoogleCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)GoogleCheckBox.IsChecked)
            {
                GoogleHeader.IsEnabled = true;
                GoogleColumn.Width = 460;
            }
            else
            {
                GoogleHeader.IsEnabled = false; ;
                GoogleColumn.Width = 0;
            }
        }
        #endregion

        #region Search
        private async void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                canClickUI.CanClick = false;
                SearchButton.Visibility = Visibility.Hidden;
                CancelButton.Visibility = Visibility.Visible;

                // Progress bar implementation.
                Progress<int> progress = new Progress<int>(value => {
                    StatusBarProgressBar.Value = value;
                });

                if (SearchComboBox.Text == "Suspicious Translations DeepL")
                {
                    await Search.Suspicious(true, progress, cancelSearch.Token);                    
                }
                else if (SearchComboBox.Text == "Suspicious Translations Google")
                {
                    await Search.Suspicious(true, progress, cancelSearch.Token);
                }
                else
                {
                    TlTypes searchMode = Tools.GetSearchMode(SearchComboBox.Text);
                    await Search.String(SearchTextBox.Text, searchMode, progress, cancelSearch.Token);
                }

                SearchButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Hidden;
                cancelSearch.Dispose();
                cancelSearch = new CancellationTokenSource();
                canClickUI.CanClick = true;
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            canClickUI.CanClick = false;
            SearchButton.Visibility = Visibility.Hidden;
            CancelButton.Visibility = Visibility.Visible;

            // Progress bar implementation.
            Progress<int> progress = new Progress<int>(value => {
                StatusBarProgressBar.Value = value;
            });

            if (SearchComboBox.Text == "Suspicious Translations DeepL")
            {
                await Search.Suspicious(true, progress, cancelSearch.Token);
            }
            else if (SearchComboBox.Text == "Suspicious Translations Google")
            {
                await Search.Suspicious(false, progress, cancelSearch.Token);
            }
            else
            {
                TlTypes searchMode = Tools.GetSearchMode(SearchComboBox.Text);
                await Search.String(SearchTextBox.Text, searchMode, progress, cancelSearch.Token);
            }

            SearchButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Hidden;
            cancelSearch.Dispose();
            cancelSearch = new CancellationTokenSource();
            canClickUI.CanClick = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelSearch.Cancel();
        }
        #endregion

        #region Database Options Buttons
        private void AutoSaveCheckBox_Click(object sender, RoutedEventArgs e)
        {
            TranslationDatabase.IsAutoSaveEnabled = (bool)AutoSaveCheckBox.IsChecked;
        }
        private async void DatabaseSaveButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => TranslationDatabase.Save());
        }
        private async void DatabaseBackupButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => TranslationDatabase.Save(true));
        }
        #endregion

        #region Various Tools Buttons

        private async void UpdateStatsButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => statusBar.UpdateStats());
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            Log.Clear();
        }

        private void SortScriptFilesButton_Click(object sender, RoutedEventArgs e)
        {
            SortWindow sortWindow = new SortWindow();
            sortWindow.Owner = this;
            sortWindow.Show();
        }

        private void DatabaseShareButton_Click(object sender, RoutedEventArgs e)
        {
            ShareWindow shareWindow = new ShareWindow();
            shareWindow.Owner = this;
            shareWindow.Show();
        }
        private void DeleteListButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteListWindow deleteListWindow = new DeleteListWindow();
            deleteListWindow.Owner = this;
            deleteListWindow.Show();
        }

        private void ReplaceStringButton_Click(object sender, RoutedEventArgs e)
        {
            ReplaceWindow replaceWindow = new ReplaceWindow();
            replaceWindow.Owner = this;
            replaceWindow.Show();
        }
        #endregion
    }
}
