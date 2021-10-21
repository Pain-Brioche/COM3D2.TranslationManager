using System;
using System.Threading.Tasks;
using System.Windows;

namespace Translation_Manager
{
    /// <summary>
    /// Interaction logic for ReplaceWindow.xaml
    /// </summary>
    public partial class ReplaceWindow : Window
    {
        public ReplaceWindow()
        {
            InitializeComponent();
        }

        private async void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            bool official = (bool)ReplaceOfficialCheckBox.IsChecked;
            bool deepl = (bool)ReplaceOfficialCheckBox.IsChecked;
            bool google = (bool)ReplaceGoogleCheckBox.IsChecked;

            if (official || deepl || google)
            {
                string oldStr = ReplaceSearchTextBox.Text;
                string newStr = ReplaceStringTextBox.Text;

                // Progress bar implementation.
                Progress<int> progress = new Progress<int>(value => {
                    ReplaceProgressBar.Value = value;
                });

                await Task.Run(() => Search.Replace(official, deepl, google, oldStr, newStr, progress));
            }
        }
    }
}
