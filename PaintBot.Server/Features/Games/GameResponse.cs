using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PaintBot.Server.Models;

namespace PaintBot.Server.Features.Games;

public record GameResponse(Guid ID, ICollection<BotResponse> Bots, DateTime DateTime)
{
    public static Expression<Func<Game, GameResponse>> FromGame { get; } = (game) =>
        new(game.ID,
            game.Bots.AsQueryable().Select(BotResponse.FromBot).ToList(),
            game.DateTime);
}
