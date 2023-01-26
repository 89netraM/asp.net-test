using PaintBot.Server.Features.Errors;
using System.ComponentModel.DataAnnotations;

namespace PaintBot.Server.Features.Pagination;

public record PaginationRequest(
    [Range(0, int.MaxValue)]
    [property: ValidationFailure(ErrorCode.PageInvalid)]
        int Page = 0,
    [Range(1, int.MaxValue)]
    [property: ValidationFailure(ErrorCode.PageSizeInvalid)]
        int PageSize = 20);
