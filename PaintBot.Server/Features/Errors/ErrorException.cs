using System;

namespace PaintBot.Server.Features.Errors;

public class ErrorException : Exception
{
    public ErrorCode ErrorCode { get; init; }
    public string? Details { get; init; }

    public ErrorException(ErrorCode errorCode, string? details = null) =>
        (ErrorCode, Details) = (errorCode, details);
}
