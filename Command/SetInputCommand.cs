using System.Windows.Input;

namespace FureaiReminder.Command
{
    internal class SetInputCommand : ICommand
    {
        private readonly ReminderViewModel viewModel;

        public SetInputCommand(ReminderViewModel view)
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
            viewModel.ConfirmInputTime();
        }
    }
}
