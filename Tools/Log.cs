using System;

namespace Translation_Manager
{
    public static class Log
    {
        public static void Write(string message)
        {
            // Make it so this can be use by other threads
            App.Current.Dispatcher.BeginInvoke((Action)delegate {
                MainWindow.LogEntries.Add(message);
            });
                    }
        public static void Clear()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate {
                MainWindow.LogEntries.Clear();
            });
        }
    }
}
