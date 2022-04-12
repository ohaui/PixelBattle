using PixelBattleAPI.Enums;

namespace PixelBattleAPI.Model;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public Privileges Privileges { get; set; } = Privileges.Default;
    public DateTime LatestChange { get; set; } = DateTime.Now.Add(new TimeSpan(0, -3, 0));
}