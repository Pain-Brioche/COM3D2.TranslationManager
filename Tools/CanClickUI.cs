using System.ComponentModel;

namespace Translation_Manager
{
    internal class CanClickUI : INotifyPropertyChanged
    {
        private bool _canClick = false;
        private bool _isSaved = true;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public bool CanClick {
            get => _canClick;
            set {
                if (_canClick == value) { return; }

                _canClick = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CanClick)));
            }
        }

        public bool IsSaved {
            get => _isSaved;
            set {
                if (_isSaved == value) { return; }

                _isSaved = value;

                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSaved)));
            }
        }
    }
}
