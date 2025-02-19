using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;

namespace FureaiReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ToastNotificationManagerCompat.OnActivated += ToastNotificationOnActivated;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (ReminderViewModel)DataContext;
            vm.LoadFromFile();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            var vm = (ReminderViewModel)DataContext;
            vm.SaveToFile();

            vm.StoreToSystemTrayCommand.DisposeNotifyIcon();
            ToastNotificationManagerCompat.Uninstall();
        }

        private void ToastNotificationOnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            var args = ToastArguments.Parse(e.Argument);
            if (args.Count == 0)
            {
                return;
            }

            this.Dispatcher.Invoke(() =>
            {
                var vm = (ReminderViewModel)DataContext;
                var action = args["action"];
                switch (action)
                {
                    case "touch":
                        vm.SetNowTime();
                        return;
                    default:
                        return;
                }
            });

        }
    }
}