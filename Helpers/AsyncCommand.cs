using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Jaezoo.Client.Wpf.Helpers;
public class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _can;
    private bool _running;
    public AsyncCommand(Func<Task> execute, Func<bool>? can = null) { _execute = execute; _can = can; }
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => !_running && (_can?.Invoke() ?? true);
    public async void Execute(object? parameter)
    {
        _running = true; CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        try { await _execute(); } finally { _running = false; CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
    }
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
