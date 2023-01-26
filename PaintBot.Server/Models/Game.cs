using System;
using System.Collections.Generic;

namespace PaintBot.Server.Models;

public class Game
{
    public Guid ID { get; set; }
    public virtual ICollection<Bot> Bots { get; set; }
    public DateTime DateTime { get; set; }
}
