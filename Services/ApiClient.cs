using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Jaezoo.Client.Wpf.Models;

namespace Jaezoo.Client.Wpf.Services;

public class ApiClient
{
    public HttpClient Http { get; }
    private readonly Session _session;

    public ApiClient(Session session)
    {
        _session = session;
        Http = new HttpClient { BaseAddress = new Uri(AppSettings.ApiBase) };
    }

    private void ApplyBearerIfAny()
    {
        if (!string.IsNullOrWhiteSpace(_session.Token))
            Http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _session.Token);
        else
            Http.DefaultRequestHeaders.Authorization = null;
    }

    // AUTH
    public async Task RegisterAsync(RegisterRequest req, CancellationToken ct = default)
    {
        var resp = await Http.PostAsJsonAsync("/api/auth/register", req, Json.Options, ct);
        if (!resp.IsSuccessStatusCode)
        {
            var msg = await resp.Content.ReadAsStringAsync(ct);
            throw new Exception($"Регистрация не удалась: {resp.StatusCode} {msg}");
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct = default)
    {
        var resp = await Http.PostAsJsonAsync("/api/auth/login", req, Json.Options, ct);
        if (!resp.IsSuccessStatusCode)
        {
            var msg = await resp.Content.ReadAsStringAsync(ct);
            throw new Exception($"Ошибка входа: {resp.StatusCode} {msg}");
        }
        var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>(Json.Options, ct)
                   ?? throw new Exception("Пустой ответ авторизации.");
        _session.Token = auth.Token;
        _session.UserId = auth.UserId;
        _session.Login = auth.Login;
        _session.Email = auth.Email;
        _session.Nickname = auth.Nickname;
        return auth;
    }

    // USERS
    public async Task<SearchUsersResponse> SearchUsersAsync(string query, CancellationToken ct = default)
    {
        ApplyBearerIfAny();
        var resp = await Http.GetAsync($"/api/users/search?q={Uri.EscapeDataString(query)}", ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Ошибка поиска: {resp.StatusCode} {await resp.Content.ReadAsStringAsync(ct)}");
        return (await resp.Content.ReadFromJsonAsync<SearchUsersResponse>(Json.Options, ct))!;
    }

    // FRIENDS
    public async Task<FriendsListResponse> GetFriendsAsync(CancellationToken ct = default)
    {
        ApplyBearerIfAny();
        var resp = await Http.GetAsync("/api/friends", ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Ошибка получения друзей: {resp.StatusCode} {await resp.Content.ReadAsStringAsync(ct)}");
        return (await resp.Content.ReadFromJsonAsync<FriendsListResponse>(Json.Options, ct))!;
    }

    public async Task SendFriendRequestAsync(int targetUserId, CancellationToken ct = default)
    {
        ApplyBearerIfAny();
        var resp = await Http.PostAsJsonAsync("/api/friends/request", new FriendRequestDto(targetUserId), Json.Options, ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Ошибка заявки в друзья: {resp.StatusCode} {await resp.Content.ReadAsStringAsync(ct)}");
    }

    public async Task RespondFriendAsync(int requesterUserId, bool accept, CancellationToken ct = default)
    {
        ApplyBearerIfAny();
        var resp = await Http.PostAsJsonAsync("/api/friends/respond", new FriendRespondDto(requesterUserId, accept), Json.Options, ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Ошибка ответа на заявку: {resp.StatusCode} {await resp.Content.ReadAsStringAsync(ct)}");
    }

    // MESSAGES
    public async Task<MessageDto> SendMessageAsync(int toUserId, string content, CancellationToken ct = default)
    {
        ApplyBearerIfAny();
        var resp = await Http.PostAsJsonAsync("/api/messages/send", new SendMessageRequest(toUserId, content), Json.Options, ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Ошибка отправки: {resp.StatusCode} {await resp.Content.ReadAsStringAsync(ct)}");
        return (await resp.Content.ReadFromJsonAsync<MessageDto>(Json.Options, ct))!;
    }

    public async Task<MessagesPage> GetDialogAsync(int userId, long? afterId = null, int limit = 50, CancellationToken ct = default)
    {
        ApplyBearerIfAny();
        var url = $"/api/messages/with/{userId}?limit={limit}";
        if (afterId is not null) url += $"&afterId={afterId}";
        var resp = await Http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Ошибка истории: {resp.StatusCode} {await resp.Content.ReadAsStringAsync(ct)}");
        return (await resp.Content.ReadFromJsonAsync<MessagesPage>(Json.Options, ct))!;
    }
}
