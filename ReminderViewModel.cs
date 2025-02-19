using System.ComponentModel;
using System.Globalization;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using FureaiReminder.Command;

namespace FureaiReminder
{
    internal class ReminderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public SetInputCommand SetInputCommand { get; private set; }
        public SetNowCommand SetNowCommand { get; private set; }
        public StoreToSystemTrayCommand StoreToSystemTrayCommand { get; private set; }


        const string TimeFormat = @"HH:mm:ss";

        const string FullTimeFormat = @"yyyy/MM/dd-HH:mm:ss";
        const string FilePath = @"Time.sav";

#if DEBUG
        const int MarginMinutes = 0;
#else
        const int MarginMinutes = 1;
#endif
        const int ConversationId = 314159265;

        public SaveTimes SaveTimes { get; set; }

        public bool IsTimerRunning
        {
            get
            {
                return timer.IsEnabled;
            }
        }
        public string StatusText
        {
            get
            {
                return IsTimerRunning ? "リマインダー稼働中" : "待機中";
            }
        }
        public string DisplayNextTouchTime
        {
            get
            {
                if (SaveTimes.NextTouchTime != null && SaveTimes.NextTouchTime.HasValue)
                {
                    return SaveTimes.NextTouchTime.Value.ToString(TimeFormat);
                }
                else
                {
                    return "--:--:--";
                }
            }
        }
        public string InputLastTouchTime { get; set; }

        private readonly DispatcherTimer timer;

        public ReminderViewModel()
        {
            SetInputCommand = new SetInputCommand(this);
            SetNowCommand = new SetNowCommand(this);
            StoreToSystemTrayCommand = new StoreToSystemTrayCommand(this);

            SaveTimes = new SaveTimes();
            InputLastTouchTime = "";

            timer = new DispatcherTimer();
            timer.Tick += TickEvent;
        }

        private void TickEvent(object? sender, EventArgs e)
        {
            if (SaveTimes.NextTouchTime != null && SaveTimes.NextTouchTime.HasValue)
            {
                var span = SaveTimes.NextTouchTime.Value - DateTime.Now;
                if (span.TotalSeconds <= 0)
                {
                    timer.Stop();

                    new ToastContentBuilder()
                        .AddArgument("action", "viewConversation")
                        .AddArgument("conversationId", ConversationId)
                        .AddText("先生、ふれあいのお時間です！")
                        .AddToastInput(new ToastSelectionBox("SnoozeTimeBox")
                        {
                            DefaultSelectionBoxItemId = "1",
                            Items =
                            {
                                new ToastSelectionBoxItem("1", "再通知 1分後"),
                                new ToastSelectionBoxItem("3", "再通知 3分後"),
                                new ToastSelectionBoxItem("5", "再通知 5分後"),
                                new ToastSelectionBoxItem("10", "再通知 10分後"),
                                new ToastSelectionBoxItem("15", "再通知 15分後"),
                            }
                        })
                        .SetToastScenario(ToastScenario.Reminder)
                        .AddButton(new ToastButton()
                                .SetContent("ふれあった！")
                                .AddArgument("action", "touch")
                            )
                        .AddButton(new ToastButtonSnooze() { SelectionBoxId = "SnoozeTimeBox" })
                        .AddButton(new ToastButtonDismiss("閉じる"))
                        .Show();
                }
                else if (span.TotalMinutes <= (MarginMinutes * 2))
                {
                    timer.Interval = TimeSpan.FromSeconds(1);
                }
            }
            else
            {
                timer.Stop();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsTimerRunning)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
        }


        public void ConfirmInputTime()
        {
            if (string.IsNullOrEmpty(InputLastTouchTime))
            {
                SetLastTouchTime(null);
            }
            else
            {
                var timeStr = (InputLastTouchTime.Length == 7) ? "0" + InputLastTouchTime : InputLastTouchTime;
                if (DateTime.TryParseExact(timeStr, TimeFormat, null, DateTimeStyles.None, out DateTime result))
                {
                    SetLastTouchTime(result);
                }
                else
                {
                    SetLastTouchTime(null);
                }
            }
        }

        public void SetNowTime()
        {
            var now = DateTime.Now;
            SetLastTouchTime(now);
            InputLastTouchTime = now.ToString(TimeFormat);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputLastTouchTime)));
        }

        private void SetLastTouchTime(DateTime? time)
        {
            if (time != null && time.HasValue)
            {
                SaveTimes.LastTouchTime = time;
                SaveTimes.NextTouchTime = CalcNextTouchTime(time.Value);

                var span = SaveTimes.NextTouchTime.Value - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    timer.Interval = span - TimeSpan.FromMinutes(MarginMinutes);
                    timer.Start();
                }
                else
                {
                    SaveTimes.LastTouchTime = null;
                    SaveTimes.NextTouchTime = null;

                    timer.Stop();
                }
            }
            else
            {
                SaveTimes.LastTouchTime = null;
                SaveTimes.NextTouchTime = null;
            }

            ToastNotificationManagerCompat.History.Clear();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayNextTouchTime)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsTimerRunning)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
        }

        private static DateTime CalcNextTouchTime(DateTime time)
        {
#if DEBUG
            return time.AddSeconds(3);
#endif

            var hour = time.Hour;
            if (1 <= hour && hour <= 3)
            {
                return new DateTime(time.Year, time.Month, time.Day, 4, 0, 0);
            }
            else if (13 <= hour && hour <= 15)
            {
                return new DateTime(time.Year, time.Month, time.Day, 16, 0, 0);
            }
            else
            {
                return time.AddHours(3);
            }
        }

        public void LoadFromFile()
        {
            if (System.IO.File.Exists(FilePath))
            {
                var text = System.IO.File.ReadAllText(FilePath);
                if (!string.IsNullOrEmpty(text))
                {
                    if (!DateTime.TryParseExact(text, FullTimeFormat, null, DateTimeStyles.None, out DateTime lastTime))
                    {
                        return;
                    }

                    var nextTime = CalcNextTouchTime(lastTime);
                    if (nextTime <= DateTime.Now)
                    {
                        return;
                    }

                    InputLastTouchTime = lastTime.ToString(TimeFormat);
                    SetLastTouchTime(lastTime);

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputLastTouchTime)));
                }
            }

        }

        public void SaveToFile()
        {
            var text = "";
            if (timer.IsEnabled)
            {
                if (SaveTimes.LastTouchTime != null && SaveTimes.LastTouchTime.HasValue)
                {
                    text = SaveTimes.LastTouchTime.Value.ToString(FullTimeFormat);
                }
            }

            System.IO.File.WriteAllText(FilePath, text);
        }
    }
}
