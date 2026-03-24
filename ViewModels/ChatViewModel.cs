using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Jaezoo.Client.Wpf.Helpers;
using Jaezoo.Client.Wpf.Models;
using Jaezoo.Client.Wpf.Services;

namespace Jaezoo.Client.Wpf.ViewModels;

public class ChatViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _api;
    private readonly Session _session;
    public event PropertyChangedEventHandler? PropertyChanged;
    void OnChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    public ObservableCollection<MessageDto> Messages { get; } = new();
    public FriendListItem? SelectedPeer { get => _peer; set { _peer = value; OnChanged(); ResetAndLoadAsync(); } }
    private FriendListItem? _peer;
    public string Outgoing { get => _out; set { _out = value; OnChanged(); } }
    private string _out = "";
    public long? LastId { get; private set; }

    public AsyncCommand SendCmd { get; }
    private readonly DispatcherTimer _pollTimer;

    public ChatViewModel(ApiClient api, Session session, Dispatcher dispatcher)
    {
        _api = api; _session = session;
        SendCmd = new AsyncCommand(SendAsync, () => SelectedPeer is not null && !string.IsNullOrWhiteSpace(Outgoing));
        _pollTimer = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Background, async (_, _) => await PollAsync(), dispatcher);
        _pollTimer.Start();
    }

    private async Task ResetAndLoadAsync()
    {
        Messages.Clear(); LastId = null;
        if (SelectedPeer is null) return;
        var page = await _api.GetDialogAsync(SelectedPeer.Id, null, 50);
        foreach (var m in page.Items) Messages.Add(m);
        LastId = page.LastId;
    }

    public async Task PollAsync()
    {
        if (SelectedPeer is null) return;
        var page = await _api.GetDialogAsync(SelectedPeer.Id, LastId, 200);
        if (page.Items.Count > 0)
        {
            foreach (var m in page.Items) Messages.Add(m);
            LastId = page.LastId;
        }
    }

    private async Task SendAsync()
    {
        if (SelectedPeer is null || string.IsNullOrWhiteSpace(Outgoing)) return;
        var sent = await _api.SendMessageAsync(SelectedPeer.Id, Outgoing.Trim());
        Messages.Add(sent);
        LastId = sent.Id;
        Outgoing = "";
    }
}
