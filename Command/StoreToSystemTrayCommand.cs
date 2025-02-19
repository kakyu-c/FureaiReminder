using System.Windows.Input;

namespace FureaiReminder.Command
{
    internal class StoreToSystemTrayCommand : ICommand
    {
        private readonly ReminderViewModel viewModel;

        private System.Windows.Forms.NotifyIcon? notifyIcon;
        private readonly System.Drawing.Icon icon;

        public StoreToSystemTrayCommand(ReminderViewModel view)
        {
            viewModel = view;
            notifyIcon = null;

            var s = System.Windows.Application.GetResourceStream(new Uri(@"Resources/bell.ico", UriKind.Relative)).Stream;
            icon = new System.Drawing.Icon(s);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            DisposeNotifyIcon();

            var menu = new ContextMenuStrip();

            var openMenu = new ToolStripMenuItem()
            {
                Text = "ウィンドウを開く",
            };
            openMenu.Click += (sender, e) =>
            {
                System.Windows.Application.Current.MainWindow.Show();
                DisposeNotifyIcon();
            };

            var endMenu = new ToolStripMenuItem()
            {
                Text = "プログラムを終了",
            };
            endMenu.Click += (sender, e) =>
            {
                System.Windows.Application.Current.Shutdown();
            };

            menu.Items.Add(openMenu);
            menu.Items.Add(endMenu);

            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = icon,
                Text = "待機中",
                ContextMenuStrip = menu
            };
            notifyIcon.DoubleClick += (sender, e) =>
            {
                System.Windows.Application.Current.MainWindow.Show();
                DisposeNotifyIcon();
            };
            notifyIcon.MouseMove += (sender, e) =>
            {
                if (viewModel.SaveTimes.NextTouchTime != null && viewModel.SaveTimes.NextTouchTime.HasValue)
                {
                    var span = viewModel.SaveTimes.NextTouchTime.Value - DateTime.Now;
                    if (span.TotalSeconds > 0)
                    {
                        notifyIcon.Text = $"あと {span.Hours}時間{span.Minutes}分{span.Seconds}秒";
                    }
                    else
                    {
                        notifyIcon.Text = "待機中";
                    }
                }
                else
                {
                    notifyIcon.Text = "待機中";
                }
            };

            System.Windows.Application.Current.MainWindow.Hide();
        }

        public void DisposeNotifyIcon()
        {
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }
        }
    }
}
