using System.Text.Json.Serialization;

namespace BugStore.Application.DTOs;

public class PagedResponse<TData> : Response<TData>
{
    [JsonConstructor]
    public PagedResponse(TData? data, int totalCount, int statusCode = Configuration.DefaultStatusCode, int currentPage = Configuration.DefaultPageNumber,
        int pageSize = Configuration.DefaultPageSize, string? message = null) : base(data, statusCode, message){
        Data = data;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }

    public PagedResponse(TData? data, int statusCode = Configuration.DefaultStatusCode, string? message = null)
        : base(data, statusCode, message){
    }

    public int CurrentPage { get; set; }
    public int TotalPages => (int) Math.Ceiling(TotalCount / (double) PageSize);
    public int PageSize { get; set; } = Configuration.DefaultPageSize;
    public int TotalCount { get; set; }
}