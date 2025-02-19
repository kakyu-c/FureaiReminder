using System.Windows.Input;

namespace FureaiReminder.Command
{
    internal class SetNowCommand : ICommand
    {
        private readonly ReminderViewModel viewModel;

        public SetNowCommand(ReminderViewModel view)
        {
            viewModel = view;
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
            viewModel.SetNowTime();
        }
    }
}
