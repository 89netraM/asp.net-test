using Microsoft.AspNetCore.Http;

namespace PaintBot.Server.Features.Errors;

public enum ErrorCode
{
	/// <summary>
	/// An internal server error occured.
	/// </summary>
	InternalServerError,
    /// <summary>
    /// Unspecified request parameters were invalid.
    /// </summary>
    ParametersInvalid,
	/// <summary>
	/// The reqeusted game could not be found.
	/// </summary>
	GameNotFound,
	/// <summary>
	/// The provided game ID is of an invalid format.
	/// </summary>
	GameIdInvalid,
    /// <summary>
    /// The requested page is invalid, either too small or too large, or of the wrong format.
    /// </summary>
    PageInvalid,
	/// <summary>
	/// The requested page size is invalid, either too small or of the wrong format.
	/// </summary>
	PageSizeInvalid,
}

public static class ErrorCodeExtension
{
#pragma warning disable CS8524
	public static int ToStatusCode(this ErrorCode errorCode) => errorCode switch
	{
		ErrorCode.InternalServerError => StatusCodes.Status500InternalServerError,
		ErrorCode.ParametersInvalid => StatusCodes.Status400BadRequest,
		ErrorCode.GameNotFound => StatusCodes.Status404NotFound,
		ErrorCode.GameIdInvalid => StatusCodes.Status400BadRequest,
		ErrorCode.PageInvalid => StatusCodes.Status400BadRequest,
		ErrorCode.PageSizeInvalid => StatusCodes.Status400BadRequest,
	};
#pragma warning restore CS8524
}
