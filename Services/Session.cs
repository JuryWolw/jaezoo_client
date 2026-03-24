namespace Jaezoo.Client.Wpf.Services;

public class Session
{
    public string? Token { get; set; }
    public int? UserId { get; set; }
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string? Nickname { get; set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    public void Clear()
    {
        Token = null; UserId = null; Login = null; Email = null; Nickname = null;
    }
}
