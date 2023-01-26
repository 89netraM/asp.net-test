using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaintBot.Server.Features.Errors;
using PaintBot.Server.Features.Pagination;
using PaintBot.Server.PastGames;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PaintBot.Server.Features.Games;

/// <summary>
/// Reading and searching for past games.
/// </summary>
[ApiController]
[Route("[controller]")]
public class GamesController : ControllerBase
{
    private readonly GamesContext context;

    public GamesController(GamesContext context) =>
        this.context = context;

    /// <summary>
    /// Lists all games.
    /// </summary>
    /// <returns>A list of games.</returns>
    /// <response code="200">Returns a list of games.</response>
    [HttpGet]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(PaginationResponse<GameResponse>), StatusCodes.Status200OK)]
    [ProducesErrorCode(ErrorCode.PageInvalid)]
    [ProducesErrorCode(ErrorCode.PageSizeInvalid)]
    public Task<PaginationResponse<GameResponse>> GetGames(
        [FromQuery]
            PaginationRequest pagination) =>
        context.Games
            .OrderByDescending(game => game.DateTime)
            .Select(GameResponse.FromGame)
            .ToPaginationResponse(pagination);

	/// <summary>
	/// Fetches one specific game.
	/// </summary>
	/// <param name="id">The ID of the game to fetch.</param>
	/// <returns>The requested game.</returns>
	/// <response code="200">Returns the requested game.</response>
	[HttpGet("{id}")]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesErrorCode(ErrorCode.GameIdInvalid)]
    [ProducesErrorCode(ErrorCode.GameNotFound)]
    public async Task<GameResponse> GetGame(
        [FromRoute, ValidationFailure(ErrorCode.GameIdInvalid)]
            Guid id) =>
        await context.Games
            .Where(game => game.ID == id)
            .Select(GameResponse.FromGame)
            .FirstOrDefaultAsync() ?? throw new ErrorException(ErrorCode.GameNotFound);
}
