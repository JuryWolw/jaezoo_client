namespace Jaezoo.Client.Wpf.Models;

public class FriendListItem
{
    public int Id { get; set; }
    public string Display => string.IsNullOrWhiteSpace(Nickname) ? Login : Nickname!;
    public string? Nickname { get; set; }
    public string Login { get; set; } = "";
}

public class UserSearchItem : FriendListItem { }
