using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System;

namespace Translation_Manager
{
    /// <summary>
    /// Interaction logic for ShareWindow.xaml
    /// </summary>
    public partial class ShareWindow : Window
    {
        private readonly ShareDatabase shareDatabase = new ShareDatabase();

        public ShareWindow()
        {
            InitializeComponent();
        }

        private async void ExportDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            bool isDeeplEnabled = (bool)ExportDeepLCheckbox.IsChecked;
            bool isGoogleEnabled = (bool)ExportGoogleCheckbox.IsChecked;

            ExportDatabaseButton.IsEnabled = false;

            // Progress bar implementation.
            Progress<int> progress = new Progress<int>(value => {
                ImportDatabaseProgressBar.Value = value;
            });

            await Task.Run(() => shareDatabase.Export(isDeeplEnabled, isGoogleEnabled, progress));
            Close();
        }

        private async void ImportDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(ImportDatabaseTextBox.Text))
            {
                bool isJapaneseImported = (bool)ImportJapaneseCheckbox.IsEnabled;
                bool isDeeplImported = (bool)ImportDeepLCheckbox.IsEnabled;
                bool isGoogleImported = (bool)ImportGoogleCheckbox.IsEnabled;
                bool overwrite = (bool)ImportOverwriteCheckbox.IsEnabled;
                string importPath = ImportDatabaseTextBox.Text;

                ImportDatabaseButton.IsEnabled = false;

                // Progress bar implementation.
                Progress<int> progress = new Progress<int>(value => {
                    ImportDatabaseProgressBar.Value = value;
                });

                Log.Write("Starting Import...");
                await Task.Run(() => shareDatabase.Import(isJapaneseImported, isDeeplImported, isGoogleImported, overwrite, importPath, progress));

                Log.Write("Database has been imported.");
                Close();
            }
            else { Log.Write("File doesn't exist."); }

        }

        private void ImportDatabaseBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImportDatabaseTextBox.Text = openFileDialog.FileName;
            }
        }
    }
}
