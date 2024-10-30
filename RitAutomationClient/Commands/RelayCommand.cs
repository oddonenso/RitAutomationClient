using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RitAutomationClient.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute ?? (() => true);
        }

        public bool CanExecute(object parameter) => _canExecute();

        public async void Execute(object parameter)
        {
            try
            {
                await _executeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при выполнении команды: " + ex.Message);
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
