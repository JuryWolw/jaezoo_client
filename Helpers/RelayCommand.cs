using System;
using System.Windows.Input;

namespace Jaezoo.Client.Wpf.Helpers;
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _can;
    public RelayCommand(Action execute) : this(_ => execute(), _ => true) { }
    public RelayCommand(Action execute, Func<bool> can) : this(_ => execute(), _ => can()) { }
    public RelayCommand(Action<object?> execute, Func<object?, bool>? can = null)
    { _execute = execute; _can = can; }
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => _can?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => _execute(parameter);
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
