using System;

namespace Jaezoo.Client.Wpf.Models;

public record RegisterRequest(string? Nickname, string Login, string Email, string Password, string ConfirmPassword);
public record LoginRequest(string LoginOrEmail, string Password);

public record AuthResponse(string Token, int UserId, string? Nickname, string Login, string Email);

public record UserSummaryDto(int Id, string? Nickname, string Login);
public record SearchUsersResponse(System.Collections.Generic.List<UserSummaryDto> Results);

public record FriendRequestDto(int TargetUserId);
public record FriendRespondDto(int RequesterUserId, bool Accept);

public record FriendsListResponse(
    System.Collections.Generic.List<UserSummaryDto> Friends,
    System.Collections.Generic.List<UserSummaryDto> IncomingPending,
    System.Collections.Generic.List<UserSummaryDto> OutgoingPending);

public record SendMessageRequest(int ToUserId, string Content);

public record MessageDto(long Id, int SenderId, int RecipientId, string Content, DateTime CreatedAtUtc);
public record MessagesPage(System.Collections.Generic.List<MessageDto> Items, long? LastId);
