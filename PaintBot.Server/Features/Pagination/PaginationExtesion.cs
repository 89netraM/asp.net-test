using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using PaintBot.Server.Features.Errors;

namespace PaintBot.Server.Features.Pagination;
public static class PaginationExtension
{
    public static Task<PaginationResponse<T>> ToPaginationResponse<T>(this IQueryable<T> source, PaginationRequest pagination) =>
            source.ToPaginationResponse(pagination.Page, pagination.PageSize);
    public static async Task<PaginationResponse<T>> ToPaginationResponse<T>(this IQueryable<T> source, int page, int pageSize)
    {
        var pageCount = await TotalPages(source, pageSize);
        if (page >= pageCount)
        {
            throw new ErrorException(ErrorCode.PageInvalid);
        }

        var items = await source.Skip(page * pageSize).Take(pageSize).ToListAsync();
        return new PaginationResponse<T>(page, pageCount, items);
    }

    private static async Task<int> TotalPages<T>(IQueryable<T> source, int pageSize) =>
        (int)MathF.Ceiling(await source.CountAsync() / (float)pageSize);
}
