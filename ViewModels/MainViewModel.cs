using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Jaezoo.Client.Wpf.Helpers;
using Jaezoo.Client.Wpf.Services;

namespace Jaezoo.Client.Wpf.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public FriendsViewModel Friends { get; }
    public SearchViewModel Search { get; }
    public ChatViewModel Chat { get; }

    public RelayCommand SelectFriendCmd { get; }
    public RelayCommand RefreshAllCmd { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    public MainViewModel(ApiClient api, Session session, Dispatcher dispatcher)
    {
        Friends = new FriendsViewModel(api);
        Search = new SearchViewModel(api);
        Chat = new ChatViewModel(api, session, dispatcher);

        SelectFriendCmd = new RelayCommand<object?>(p =>
        {
            if (p is Models.FriendListItem item) Chat.SelectedPeer = item;
        });
        RefreshAllCmd = new RelayCommand(async () => await RefreshAsync());
    }

    public async Task RefreshAsync()
    {
        await Friends.RefreshAsync();
        // если выбранный собеседник есть, чат сам доползет через Poll
    }
}
