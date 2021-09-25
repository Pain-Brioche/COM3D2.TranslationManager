using System.Threading.Tasks;
using System.Windows;
using WinForm = System.Windows.Forms;
using System.IO;
using System;

namespace Translation_Manager
{
    /// <summary>
    /// Interaction logic for SortWindow.xaml
    /// </summary>
    public partial class SortWindow : Window
    {
        public WinForm.FolderBrowserDialog folderBrowserDialog = new WinForm.FolderBrowserDialog();

        public SortWindow()
        {
            InitializeComponent();
        }

        private void SortScriptBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == WinForm.DialogResult.OK)
            {
                SortScriptPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void SortOKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(SortScriptPathTextBox.Text))
            {
                Log.Write("Sorting files, this could take a while...");
                SortOKButton.IsEnabled = false;
                string sortPath = SortScriptPathTextBox.Text;

                // Progress bar implementation.
                Progress<int> progress = new Progress<int>(value => {
                    SortProgressBar.Value = value;
                });

                await Task.Run(() => Sort.Scripts(sortPath, progress));

                Close();
                Log.Write("Sorting Done!");
            }
            else
            {
                Log.Write("Selected folder doesn't exist");
            }
        }
    }
}
