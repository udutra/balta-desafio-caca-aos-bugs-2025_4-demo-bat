namespace BugStore.Application.Requests;

public abstract class PagedRequest(int pageNumber = Configuration.DefaultPageNumber,
    int pageSize = Configuration.DefaultPageSize){

    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}