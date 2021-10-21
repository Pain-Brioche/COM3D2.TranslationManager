using System;
using System.Threading.Tasks;
using System.Windows;

namespace Translation_Manager
{
    /// <summary>
    /// Interaction logic for DeleteListWindow.xaml
    /// </summary>
    public partial class DeleteListWindow : Window
    {
        public DeleteListWindow()
        {
            InitializeComponent();
        }

        private async void DeleteEntriesOKButton_Click(object sender, RoutedEventArgs e)
        {
            bool official = (bool)DeleteOfficialEntriesCheckBox.IsChecked;
            bool deepl = (bool)DeleteDeeplEntriesCheckBox.IsChecked;
            bool google = (bool)DeleteGoogleEntriesCheckBox.IsChecked;

            if (official || deepl || google)
            {
                // Progress bar implementation.
                Progress<int> progress = new Progress<int>(value => {
                    DeleteEntriesProgressBar.Value = value;
                });

                await Task.Run(() => Search.Delete(official, deepl, google, progress));
            }
            Log.Write("Entries have been delete.");
            Log.Write("Please refresh your Right Panel.");
        }
    }
}
