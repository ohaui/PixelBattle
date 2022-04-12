using System.ComponentModel.DataAnnotations;

namespace PixelBattleAPI.Records;

public record RegisterRecord()
{
    [Required]
    [StringLength(20)]
    public string Username { get; set; }

    [Required]
    [StringLength(20)]
    public string Password { get; set; }
}