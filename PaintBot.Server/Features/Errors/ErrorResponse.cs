using System.Text.Json.Serialization;

namespace PaintBot.Server.Features.Errors;

public record ErrorResponse(
        ErrorCode Code,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string? Details = null);
