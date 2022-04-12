using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PixelBattleAPI.Model;

public class Pixel
{
    [Key]
    public int Position { get; set; }
    public User? UserOwn { get; set; }
    public int Color { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
}