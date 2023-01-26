using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PaintBot.Server.Models;

[PrimaryKey(nameof(Name))]
public class Bot
{
    public string Name { get; set; }
    public virtual ICollection<Game> Games { get; set; }
}
