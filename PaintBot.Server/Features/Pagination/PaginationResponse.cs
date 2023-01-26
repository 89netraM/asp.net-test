using System.Collections.Generic;

namespace PaintBot.Server.Features.Pagination;

public record PaginationResponse<T>(int Page, int PageCount, ICollection<T> Items);
