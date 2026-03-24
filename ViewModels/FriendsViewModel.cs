using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Jaezoo.Client.Wpf.Helpers;
using Jaezoo.Client.Wpf.Models;
using Jaezoo.Client.Wpf.Services;

namespace Jaezoo.Client.Wpf.ViewModels;

public class FriendsViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;
    public ObservableCollection<FriendListItem> Friends { get; } = new();
    public ObservableCollection<FriendListItem> Incoming { get; } = new();
    public ObservableCollection<FriendListItem> Outgoing { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    public AsyncCommand RefreshCmd { get; }
    public AsyncCommand<int> AcceptCmd { get; }
    public AsyncCommand<int> DeclineCmd { get; }

    public FriendsViewModel(ApiClient api)
    {
        _api = api;
        RefreshCmd = new AsyncCommand(RefreshAsync);
        AcceptCmd = new AsyncCommand<int>(AcceptAsync);
        DeclineCmd = new AsyncCommand<int>(DeclineAsync);
    }

    public async Task RefreshAsync()
    {
        var r = await _api.GetFriendsAsync();
        Friends.SyncWith(r.Friends.Select(u => new FriendListItem { Id = u.Id, Nickname = u.Nickname, Login = u.Login }));
        Incoming.SyncWith(r.IncomingPending.Select(u => new FriendListItem { Id = u.Id, Nickname = u.Nickname, Login = u.Login }));
        Outgoing.SyncWith(r.OutgoingPending.Select(u => new FriendListItem { Id = u.Id, Nickname = u.Nickname, Login = u.Login }));
    }

    private async Task AcceptAsync(int requesterId)
    {
        await _api.RespondFriendAsync(requesterId, true);
        await RefreshAsync();
    }
    private async Task DeclineAsync(int requesterId)
    {
        await _api.RespondFriendAsync(requesterId, false);
        await RefreshAsync();
    }
}

// маленькое расширение для удобной синхронизации ObservableCollection
static class ObservableCollectionExt
{
    public static void SyncWith<T>(this ObservableCollection<T> col, System.Collections.Generic.IEnumerable<T> items)
    {
        col.Clear();
        foreach (var i in items) col.Add(i);
    }
}

// обобщенная AsyncCommand с параметром:
public class AsyncCommand<T> : AsyncCommand
{
    private readonly System.Func<T?, System.Threading.Tasks.Task> _execParam;
    public AsyncCommand(System.Func<T?, System.Threading.Tasks.Task> exec) : base(() => System.Threading.Tasks.Task.CompletedTask)
    { _execParam = exec; }
    public async void Execute(object? parameter) => await _execParam(parameter is T t ? t : default);
}
