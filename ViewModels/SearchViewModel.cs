using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Jaezoo.Client.Wpf.Helpers;
using Jaezoo.Client.Wpf.Models;
using Jaezoo.Client.Wpf.Services;

namespace Jaezoo.Client.Wpf.ViewModels;

public class SearchViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;
    public event PropertyChangedEventHandler? PropertyChanged;
    void OnChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    public string Query { get => _q; set { _q = value; OnChanged(); } }
    private string _q = "";

    public ObservableCollection<UserSearchItem> Results { get; } = new();

    public AsyncCommand SearchCmd { get; }
    public AsyncCommand<int> AddFriendCmd { get; }

    public SearchViewModel(ApiClient api)
    {
        _api = api;
        SearchCmd = new AsyncCommand(SearchAsync, () => !string.IsNullOrWhiteSpace(Query) && Query.Trim().Length >= 2);
        AddFriendCmd = new AsyncCommand<int>(AddFriendAsync);
    }

    public async Task SearchAsync()
    {
        var r = await _api.SearchUsersAsync(Query.Trim());
        Results.SyncWith(r.Results.Select(u => new UserSearchItem { Id = u.Id, Nickname = u.Nickname, Login = u.Login }));
    }

    private async Task AddFriendAsync(int userId)
    {
        await _api.SendFriendRequestAsync(userId);
        // опционально — обновим результат: пометим, что заявка отправлена (упрощенно не делаем)
    }
}
