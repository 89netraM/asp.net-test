using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PaintBot.Server.Models;

namespace PaintBot.Server.Features.Games;

public record BotResponse(string Name, ICollection<Guid> Games)
{
    public static Expression<Func<Bot, BotResponse>> FromBot { get; } = (bot) =>
        new(bot.Name,
            bot.Games.AsQueryable().Select(g => g.ID).ToList());
}
